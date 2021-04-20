using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImoutoDesktop.Scripting
{
    /// <summary>
    /// 文法の構文解析器です。
    /// </summary>
    class Parser
    {
        /// <summary>
        /// 構文解析エンジンを初期化します。
        /// </summary>
        public Parser()
        {
            encoding = Encoding.UTF8;
            globalDefines = new List<Define>();
        }

        private Encoding encoding;

        /// <summary>
        /// 読み込みに使用する文字エンコーディングを取得、設定します。
        /// </summary>
        public Encoding Encoding
        {
            set { encoding = value; }
        }

        private List<Define> globalDefines;

        /// <summary>
        /// 指定された辞書を解析し、関数を作成します。
        /// </summary>
        /// <param name="path">辞書のパス。</param>
        /// <param name="functions">関数のコレクション。</param>
        public void ParseDictionary(string path, Functions functions)
        {
            var dicname = Path.GetFileName(path);
            using (var reader = new DictionaryReader(path, encoding))
            {
                var isCommonBody = false;
                Function function = null;
                var defines = new List<Define>();
                var commonExpressions = new List<IExpression>();
                while (reader.Peek() != -1)
                {
                    // 1行読み取ってインデントを削除
                    var line = reader.ReadLine().Trim();
                    // コメント、空白文字を処理する
                    if (string.Compare(line, 0, "//", 0, 2) == 0 || line.Length == 0)
                    {
                        continue;
                    }
                    // ブレスならば中をすべて連結する
                    if (line == "{")
                    {
                        line = CombineInBrace(reader);
                        if (line.Length == 0)
                        {
                            continue;
                        }
                    }
                    // プリプロセッサを処理する
                    if (line[0] == '#')
                    {
                        if (line == "#Common")
                        {
                            // 取得する
                            function = null;
                            isCommonBody = true;
                        }
                        else if (line == "#Define")
                        {
                            var define = new Define("", "");
                            defines.Add(define);
                        }
                        else if (line == "#GlobalDefine")
                        {
                            var define = new Define("", "");
                            globalDefines.Add(define);
                        }
                        continue;
                    }
                    // プリプロセッサを処理
                    line = ExecPreProcessor(line, defines);
                    // モードを判別する
                    if (line[0] == '$')
                    {
                        // 関数定義
                        function = MakeFunction(line, functions);
                        // 辞書名を追加
                        function.DictionaryName = dicname;
                        // 辞書中の行を取得
                        function.DictionaryLine = reader.CurrentLine;
                        // プリプロセス式を追加する
                        function.Expressions.AddRange(commonExpressions);
                    }
                    else if (function != null)
                    {
                        // 関数ステートメント
                        var functionStatements = MakeStatements(line);
                        function.Statements.Add(functionStatements);
                    }
                    else if (isCommonBody)
                    {
                        isCommonBody = false;
                        var expressions = MakeStatements(line);
                        commonExpressions.AddRange(expressions);
                    }
                }
            }
        }

        private static string CombineInBrace(DictionaryReader reader)
        {
            var depth = 1;
            var result = new StringBuilder();
            while (reader.Peek() != -1)
            {
                // 1行読み取ってインデントを削除
                var line = reader.ReadLine().Trim();
                // コメント、空白文字を処理する
                if (string.Compare(line, 0, "//", 0, 2) == 0 || line.Length == 0)
                {
                    continue;
                }
                else if (line == "{")
                {
                    ++depth;
                }
                else if (line == "}")
                {
                    --depth;
                    if (depth == 0)
                    {
                        break;
                    }
                }
                result.Append(line);
            }
            if (result.Length != 0 && (result[0] == '$' || result[0] == '@'))
            {
                result.Insert(0, "{");
                result.Append("}");
            }
            return result.ToString();
        }

        private string ExecPreProcessor(string line, List<Define> define)
        {
            var length = define.Count;
            for (var i = 0; i < length; i++)
            {
                line = define[i].Before.Replace(line, define[i].After);
            }
            length = globalDefines.Count;
            for (var i = 0; i < length; i++)
            {
                line = globalDefines[i].Before.Replace(line, globalDefines[i].After);
            }
            return line;
        }

        private static string RemoveScope(string str)
        {
            if (Lexer.IsEvaluate(str))
            {
                return str.Substring(1, str.Length - 2).Trim();
            }
            return str;
        }

        private static string RemoveDoubleQuote(string str)
        {
            return str.Substring(1, str.Length - 2);
        }

        private Function MakeFunction(string line, Functions functions)
        {
            var function = new Function();
            var statement = Lexer.MakeFunctionStatement(Lexer.SplitStatement(line));
            var length = statement.Length;
            for (var i = 1; i < length; i++)
            {
                if (statement[i] == "nonoverlap")
                {
                    function.SelectType = SelectType.Nonoverlap;
                }
                else if (statement[i] == "sequential")
                {
                    function.SelectType = SelectType.Sequential;
                }
                else if (statement[i] == "array")
                {
                    function.SelectType = SelectType.Array;
                }
                else if (statement[i] == "void")
                {
                    function.SelectType = SelectType.Void;
                }
                else
                {
                    var expression = MakeExpression(statement[i]);
                    function.Expressions.Add(expression);
                }
            }
            functions.AddFunction(statement[0].Substring(1), function);
            return function;
        }

        public IExpression MakeExpression(string line)
        {
            IExpression statement = null;
            var token = RemoveScope(line);
            // トークンに分解する
            var tokens = Lexer.SplitToken(token);
            // トークンの種類を判別する
            if (tokens[0] == "$if")
            {
                // if文
                var value = Lexer.MakeIfAndWhileStatement(tokens);
                var ifexpressions = new List<IfSubExpression>();
                var length = value.Length;
                for (var i = 0; i < length; i += 3)
                {
                    IfSubExpression ifexpression;
                    if (value[i] == "$if" || value[i] == "elseif")
                    {
                        var cells = Lexer.SplitToken(value[i + 1]);
                        IExpression[] statements;
                        var expression = MakeExpression(cells);
                        if (i + 2 < length)
                        {
                            statements = MakeStatements(RemoveScope(value[i + 2]));
                        }
                        else
                        {
                            statements = new IExpression[0];
                        }
                        ifexpression = new IfSubExpression(expression, statements);
                    }
                    else if (value[i] == "else")
                    {
                        // 常に成り立つ条件式をつける
                        IExpression[] statements;
                        IExpression expression = new ValueExpression(new Value(true));
                        if (i + 1 < length)
                        {
                            statements = MakeStatements(RemoveScope(value[i + 1]));
                        }
                        else
                        {
                            statements = new IExpression[0];
                        }
                        ifexpression = new IfSubExpression(expression, statements);
                    }
                    else
                    {
                        // パースエラー : if ～ elseif ～ elseの対応がおかしい
                        continue;
                    }
                    // コレクションに追加
                    ifexpressions.Add(ifexpression);
                }
                statement = new IfExpression(ifexpressions.ToArray());
            }
            else if (tokens[0] == "$for")
            {
                // for文
                var expressions = new IExpression[3];
                var value = Lexer.MakeForAndForeachStatement(tokens);
                for (var i = 0; i < 3; i++)
                {
                    var cells = Lexer.SplitToken(value[i + 1]);
                    expressions[i] = MakeExpression(cells);
                }
                var statements = MakeStatements(RemoveScope(value[4]));
                statement = new ForExpression(expressions, statements);
            }
            else if (tokens[0] == "$foreach")
            {
                // foreach文
                var expressions = new IExpression[3];
                var value = Lexer.MakeForAndForeachStatement(tokens);
                for (var i = 0; i < 2; i++)
                {
                    var cells = Lexer.SplitToken(value[i + 1]);
                    expressions[i] = MakeExpression(cells);
                }
                var statements = MakeStatements(RemoveScope(value[3]));
                statement = new ForeachExpression(expressions, statements);
            }
            else if (tokens[0] == "$while")
            {
                // while文
                var value = Lexer.MakeIfAndWhileStatement(tokens);
                var cells = Lexer.SplitToken(RemoveScope(value[1]));
                var expression = MakeExpression(cells);
                // ステートメントを作成する
                var statements = MakeStatements(RemoveScope(value[2]));
                statement = new WhileExpression(expression, statements);
            }
            else if (tokens[0] == "$switch")
            {
                // switch文
            }
            else
            {
                // 式文
                var name = tokens[0];
                // 引数を作成する
                var arguments = Lexer.MakeFunctionArguments(tokens);
                if (arguments != null)
                {
                    var length = arguments.Length;
                    var expressions = new IExpression[length];
                    for (var i = 0; i < length; i++)
                    {
                        if (arguments[i].Length != 0)
                        {
                            var cells = Lexer.SplitToken(arguments[i]);
                            expressions[i] = MakeExpression(cells);
                        }
                        else
                        {
                            expressions[i] = new ValueExpression(new Value());
                        }
                    }
                    statement = new FunctionExpression(name, expressions);
                }
                else
                {
                    if (Lexer.IsExpression(tokens))
                    {
                        statement = MakeExpression(tokens);
                    }
                    else if (Lexer.IsVariable(token))
                    {
                        statement = new EvaluateExpression(token);
                    }
                    else
                    {
                        statement = new TextExpression("{" + token + "}");
                    }
                }
            }
            return statement;
        }

        private IExpression[] MakeStatements(string line)
        {
            // 関数ステートメント
            var statement = Lexer.SplitStatement(line);
            var statements = new IExpression[statement.Length];
            var length = statements.Length;
            for (var i = 0; i < length; ++i)
            {
                if (Lexer.IsEvaluate(statement[i]))
                {
                    // 括弧内を評価する
                    statements[i] = MakeExpression(statement[i]);
                }
                else if (Lexer.IsString(statement[i]))
                {
                    // クオート済みテキスト
                    statements[i] = new TextExpression(RemoveDoubleQuote(statement[i]));
                }
                else
                {
                    // プレーンテキスト
                    statements[i] = new TextExpression(statement[i]);
                }
            }
            return statements;
        }

        /// <summary>
        /// トークン列から式オブジェクトを作成する。
        /// </summary>
        /// <param name="tokens">トークン列。</param>
        /// <returns>作成した式オブジェクト。</returns>
        public IExpression MakeExpression(string[] tokens)
        {
            var index = 0;
            return MakeExpression0(tokens, ref index);
        }

        /// <summary>
        /// 論理和を処理する
        /// </summary>
        /// <param name="tokens">セル</param>
        /// <param name="index">現在のセルのインデックス</param>
        /// <returns>作成したステートメント</returns>
        private IExpression MakeExpression0(string[] tokens, ref int index)
        {
            var lhs = MakeExpression1(tokens, ref index);
            if (lhs == null)
            {
                return null;
            }
            while (true)
            {
                if (tokens[index] == "||")
                {
                    ++index;
                    var rhs = MakeExpression1(tokens, ref index);
                    if (rhs == null)
                    {
                        return lhs;
                    }
                    lhs = new LogicalOrExpression(lhs, rhs);
                }
                else
                {
                    return lhs;
                }
            }
        }

        /// <summary>
        /// 論理積を処理する
        /// </summary>
        /// <param name="tokens">セル</param>
        /// <param name="index">現在のセルのインデックス</param>
        /// <returns>作成したステートメント</returns>
        private IExpression MakeExpression1(string[] tokens, ref int index)
        {
            var lhs = MakeExpression2(tokens, ref index);
            if (lhs == null)
            {
                return null;
            }
            while (true)
            {
                if (tokens[index] == "&&")
                {
                    ++index;
                    var rhs = MakeExpression2(tokens, ref index);
                    if (rhs == null)
                    {
                        return lhs;
                    }
                    lhs = new LogicalAndExpression(lhs, rhs);
                }
                else
                {
                    return lhs;
                }
            }
        }

        /// <summary>
        /// 等式を処理する
        /// </summary>
        /// <param name="tokens">セル</param>
        /// <param name="index">現在のセルのインデックス</param>
        /// <returns>作成したステートメント</returns>
        private IExpression MakeExpression2(string[] tokens, ref int index)
        {
            var lhs = MakeExpression3(tokens, ref index);
            if (lhs == null)
            {
                return null;
            }
            if (tokens[index] == "=")
            {
                // 代入
                ++index;
                var rhs = MakeExpression3(tokens, ref index);
                if (rhs == null)
                {
                    return lhs;
                }
                return new SubstituteExpression(lhs, rhs);
            }
            else if (tokens[index] == "==")
            {
                // 等号
                ++index;
                var rhs = MakeExpression3(tokens, ref index);
                if (rhs == null)
                {
                    return lhs;
                }
                return new EqualExpression(lhs, rhs);
            }
            else if (tokens[index] == "!=")
            {
                // 等号
                ++index;
                var rhs = MakeExpression3(tokens, ref index);
                if (rhs == null)
                {
                    return lhs;
                }
                return new NotEqualExpression(lhs, rhs);
            }
            else if (tokens[index] == "+=")
            {
                // 等号
                ++index;
                var rhs = MakeExpression3(tokens, ref index);
                if (rhs == null)
                {
                    return lhs;
                }
                return new SubstituteAddExpression(lhs, rhs);
            }
            else if (tokens[index] == "-=")
            {
                // 等号
                ++index;
                var rhs = MakeExpression3(tokens, ref index);
                if (rhs == null)
                {
                    return lhs;
                }
                return new SubstituteSubExpression(lhs, rhs);
            }
            else if (tokens[index] == "*=")
            {
                // 等号
                ++index;
                var rhs = MakeExpression3(tokens, ref index);
                if (rhs == null)
                {
                    return lhs;
                }
                return new SubstituteMulExpression(lhs, rhs);
            }
            else if (tokens[index] == "/=")
            {
                // 等号
                ++index;
                var rhs = MakeExpression3(tokens, ref index);
                if (rhs == null)
                {
                    return lhs;
                }
                return new SubstituteDivExpression(lhs, rhs);
            }
            return lhs;
        }

        /// <summary>
        /// 比較式を処理する
        /// </summary>
        /// <param name="tokens">セル</param>
        /// <param name="index">現在のセルのインデックス</param>
        /// <returns>作成したステートメント</returns>
        private IExpression MakeExpression3(string[] tokens, ref int index)
        {
            var lhs = MakeExpression4(tokens, ref index);
            if (lhs == null)
            {
                return null;
            }
            if (tokens[index] == "<")
            {
                ++index;
                var rhs = MakeExpression4(tokens, ref index);
                if (rhs == null)
                {
                    return lhs;
                }
                return new LessExpression(lhs, rhs);
            }
            else if (tokens[index] == "<=")
            {
                ++index;
                var rhs = MakeExpression4(tokens, ref index);
                if (rhs == null)
                {
                    return lhs;
                }
                return new LessEqualExpression(lhs, rhs);
            }
            else if (tokens[index] == ">")
            {
                ++index;
                var rhs = MakeExpression4(tokens, ref index);
                if (rhs == null)
                {
                    return lhs;
                }
                return new GreaterExpression(lhs, rhs);
            }
            else if (tokens[index] == ">=")
            {
                ++index;
                var rhs = MakeExpression4(tokens, ref index);
                if (rhs == null)
                {
                    return lhs;
                }
                return new GreaterEqualExpression(lhs, rhs);
            }
            return lhs;
        }

        private IExpression MakeExpression4(string[] tokens, ref int index)
        {
            var lhs = MakeExpression5(tokens, ref index);
            if (lhs == null)
            {
                return null;
            }
            while (true)
            {
                if (tokens[index] == "|")
                {
                    ++index;
                    var rhs = MakeExpression5(tokens, ref index);
                    if (rhs == null)
                    {
                        return lhs;
                    }
                    lhs = new BitwiseOrExpression(lhs, rhs);
                }
                else if (tokens[index] == "^")
                {
                    ++index;
                    var rhs = MakeExpression5(tokens, ref index);
                    if (rhs == null)
                    {
                        return lhs;
                    }
                    lhs = new BitwiseXorExpression(lhs, rhs);
                }
                else if (tokens[index] == "<<")
                {
                    ++index;
                    var rhs = MakeExpression5(tokens, ref index);
                    if (rhs == null)
                    {
                        return lhs;
                    }
                    lhs = new ShiftLeftExpression(lhs, rhs);
                }
                else if (tokens[index] == ">>")
                {
                    ++index;
                    var rhs = MakeExpression5(tokens, ref index);
                    if (rhs == null)
                    {
                        return lhs;
                    }
                    lhs = new ShiftRightExpression(lhs, rhs);
                }
                else
                {
                    return lhs;
                }
            }
        }

        private IExpression MakeExpression5(string[] tokens, ref int index)
        {
            var lhs = MakeExpression6(tokens, ref index);
            if (lhs == null)
            {
                return null;
            }
            while (true)
            {
                if (tokens[index] == "&")
                {
                    ++index;
                    var rhs = MakeExpression6(tokens, ref index);
                    if (rhs == null)
                    {
                        return lhs;
                    }
                    lhs = new BitwiseAndExpression(lhs, rhs);
                }
                else
                {
                    return lhs;
                }
            }
        }

        /// <summary>
        /// 加減算を処理する
        /// </summary>
        /// <param name="tokens">セル</param>
        /// <param name="index">現在のセルのインデックス</param>
        /// <returns>作成したステートメント</returns>
        private IExpression MakeExpression6(string[] tokens, ref int index)
        {
            var lhs = MakeExpression7(tokens, ref index);
            if (lhs == null)
            {
                return null;
            }
            while (true)
            {
                if (tokens[index] == "+")
                {
                    ++index;
                    var rhs = MakeExpression7(tokens, ref index);
                    if (rhs == null)
                    {
                        return lhs;
                    }
                    lhs = new AddExpression(lhs, rhs);
                }
                else if (tokens[index] == "-")
                {
                    ++index;
                    var rhs = MakeExpression7(tokens, ref index);
                    if (rhs == null)
                    {
                        return lhs;
                    }
                    lhs = new SubExpression(lhs, rhs);
                }
                else
                {
                    return lhs;
                }
            }
        }

        /// <summary>
        /// 乗除算を処理する
        /// </summary>
        /// <param name="tokens">セル</param>
        /// <param name="index">現在のセルのインデックス</param>
        /// <returns>作成したステートメント</returns>
        private IExpression MakeExpression7(string[] tokens, ref int index)
        {
            var lhs = MakeExpression8(tokens, ref index);
            if (lhs == null)
            {
                return null;
            }
            while (true)
            {
                if (tokens[index] == "*")
                {
                    ++index;
                    var rhs = MakeExpression8(tokens, ref index);
                    if (rhs == null)
                    {
                        return lhs;
                    }
                    lhs = new MulExpression(lhs, rhs);
                }
                else if (tokens[index] == "/")
                {
                    ++index;
                    var rhs = MakeExpression8(tokens, ref index);
                    if (rhs == null)
                    {
                        return lhs;
                    }
                    lhs = new DivExpression(lhs, rhs);
                }
                else if (tokens[index] == "%")
                {
                    ++index;
                    var rhs = MakeExpression8(tokens, ref index);
                    if (rhs == null)
                    {
                        return lhs;
                    }
                    lhs = new ModExpression(lhs, rhs);
                }
                else
                {
                    return lhs;
                }
            }
        }

        /// <summary>
        /// 単項演算子を処理する
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private IExpression MakeExpression8(string[] tokens, ref int index)
        {
            if (tokens[index] == "+")
            {
                ++index;
                var lhs = MakeExpression8(tokens, ref index);
                if (lhs == null)
                {
                    return lhs;
                }
                return new UnaryAddExpression(lhs);
            }
            else if (tokens[index] == "-")
            {
                ++index;
                var lhs = MakeExpression8(tokens, ref index);
                if (lhs == null)
                {
                    return lhs;
                }
                return new UnarySubExpression(lhs);
            }
            else if (tokens[index] == "!")
            {
                ++index;
                var lhs = MakeExpression8(tokens, ref index);
                if (lhs == null)
                {
                    return lhs;
                }
                return new UnaryNotExpression(lhs);
            }
            else if (tokens[index] == "~")
            {
                ++index;
                var lhs = MakeExpression8(tokens, ref index);
                if (lhs == null)
                {
                    return lhs;
                }
                return new UnaryCompExpression(lhs);
            }
            return MakeExpression9(tokens, ref index);
        }

        /// <summary>
        /// 累乗を処理する
        /// </summary>
        /// <param name="tokens">セル</param>
        /// <param name="index">現在のセルのインデックス</param>
        /// <returns>作成したステートメント</returns>
        private IExpression MakeExpression9(string[] tokens, ref int index)
        {
            var lhs = MakeExpression10(tokens, ref index);
            if (lhs == null)
            {
                return null;
            }
            while (true)
            {
                if (tokens[index] == "**")
                {
                    ++index;
                    var rhs = MakeExpression10(tokens, ref index);
                    if (rhs == null)
                    {
                        return lhs;
                    }
                    lhs = new PowExpression(lhs, rhs);
                }
                else
                {
                    return lhs;
                }
            }
        }

        /// <summary>
        /// 単項インクリメント・デクリメントを処理します。
        /// </summary>
        /// <param name="tokens">セル</param>
        /// <param name="index">現在のセルのインデックス</param>
        /// <returns>作成したステートメント</returns>
        private IExpression MakeExpression10(string[] tokens, ref int index)
        {
            var lhs = MakeExpression11(tokens, ref index);
            if (lhs == null)
            {
                return null;
            }
            while (true)
            {
                if (tokens[index] == "++")
                {
                    ++index;
                    IExpression rhs = new ValueExpression(new Value(1));
                    lhs = new SubstituteAddExpression(lhs, rhs);
                }
                else if (tokens[index] == "--")
                {
                    ++index;
                    IExpression rhs = new ValueExpression(new Value(1));
                    lhs = new SubstituteSubExpression(lhs, rhs);
                }
                else
                {
                    return lhs;
                }
            }
        }

        private IExpression MakeExpression11(string[] tokens, ref int index)
        {
            IExpression expression = null;
            if (tokens[index] == "(")
            {
                ++index;
                expression = MakeExpression0(tokens, ref index);
                if (expression == null)
                {
                    return null;
                }
                if (tokens[index] == ")")
                {
                    ++index;
                }
                else
                {
                    // パースエラー : 括弧が正しく閉じられていない
                }
            }
            else
            {
                int i_value;
                bool b_value;
                double d_value;
                if (Lexer.IsVariable(tokens[index]))
                {
                    // Not Evaluate
                    expression = new VariableExpression(tokens[index]);
                }
                else if (Lexer.IsEvaluate(tokens[index]))
                {
                    // Evaluate
                    expression = MakeExpression(tokens[index]);
                }
                else if (bool.TryParse(tokens[index], out b_value))
                {
                    // Boolean
                    expression = new ValueExpression(new Value(b_value));
                }
                else if (int.TryParse(tokens[index], out i_value))
                {
                    // Int32
                    expression = new ValueExpression(new Value(i_value));
                }
                else if (double.TryParse(tokens[index], out d_value))
                {
                    // Double
                    expression = new ValueExpression(new Value(d_value));
                }
                else if (Lexer.IsBinInt32(tokens[index]))
                {
                    // Bin Int32
                    expression = new ValueExpression(new Value(Convert.ToInt32(tokens[index].Substring(2), 2)));
                }
                else if (Lexer.IsHexInt32(tokens[index]))
                {
                    // Hex Int32
                    expression = new ValueExpression(new Value(Convert.ToInt32(tokens[index].Substring(2), 16)));
                }
                else if (Lexer.IsString(tokens[index]))
                {
                    // String
                    expression = new ValueExpression(new Value(RemoveDoubleQuote(tokens[index])));
                }
                else
                {
                    // String
                    expression = new ValueExpression(new Value(tokens[index]));
                }
                ++index;
            }
            return expression;
        }
    }
}
