using System;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace ImoutoDesktop.Scripting;

internal class Lexer
{
    private Lexer()
    {
    }

    private static readonly FrozenSet<char> s_opcode = new[]
    {
        '!', '%', '&', '(', ')', '*', '+', ',',
        '-', '/', ';', '<', '=', '>', '^', '|'
    }.ToFrozenSet();

    private static readonly FrozenSet<string> s_opstring = new[]
    {
        "--", "!=", "&&", "*=", "/=", "||", "++",
        "+=", "<<", "<=", "-=", "==", ">=", ">>"
    }.ToFrozenSet(StringComparer.Ordinal);

    private static readonly FrozenSet<string> s_opsubst = new[]
    {
        "*=", "/=", "+=", "=", "-="
    }.ToFrozenSet(StringComparer.Ordinal);

    private static readonly FrozenSet<string> s_keyword = new[]
    {
        "break", "case", "default", "else", "elseif",
        "for", "foreach", "if", "switch", "while"
    }.ToFrozenSet(StringComparer.Ordinal);

    public static bool IsOperator(char c) => s_opcode.Contains(c);

    public static bool IsOperator(string str) => s_opstring.Contains(str);

    public static bool IsKeyword(string str) => s_keyword.Contains(str);

    public static bool IsEvaluate(string str) => str.StartsWith("{") && str.EndsWith("}");

    public static bool IsString(string str) => str.StartsWith("\"") && str.EndsWith("\"");

    public static bool IsBinInt32(string str) => str.StartsWith("0b");

    public static bool IsHexInt32(string str) => str.StartsWith("0x");

    public static bool IsExpression(string[] tokens)
    {
        var length = tokens.Length;
        for (var i = 0; i < length; i++)
        {
            if (s_opsubst.Contains(tokens[i]))
            {
                return true;
            }

            if (s_opstring.Contains(tokens[i]))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsVariable(string str) => str.StartsWith("$") || str.StartsWith("@");

    public static string[] SplitToken(string line)
    {
        var index = 0;
        var previndex = 0;
        var depth = 0;
        var length = line.Length;
        string temp;
        var isDoubleQuote = false;
        var token = new List<string>();
        while (index < length)
        {
            if (line[index] == '\"')
            {
                if (index == 0 || line[index - 1] != '\\')
                {
                    isDoubleQuote = !isDoubleQuote;
                }
            }
            else if (!isDoubleQuote)
            {
                if (line[index] == '{')
                {
                    if (depth == 0)
                    {
                        temp = line.Substring(previndex, index - previndex).Trim();
                        if (temp.Length != 0)
                        {
                            if (IsKeyword(temp))
                            {
                                token.Add(temp);
                                previndex = index;
                            }
                        }
                    }
                    ++depth;
                }
                else if (line[index] == '}')
                {
                    --depth;
                    if (depth == 0)
                    {
                        temp = line.Substring(previndex, index - previndex + 1).Trim();
                        if (temp.Length != 0)
                        {
                            token.Add(temp);
                            previndex = index + 1;
                        }
                    }
                }
                else if (depth == 0 && IsOperator(line[index]))
                {
                    if (previndex != index)
                    {
                        temp = line.Substring(previndex, index - previndex).Trim();
                        if (temp.Length != 0)
                        {
                            token.Add(temp);
                        }
                    }
                    if (index + 1 < length && IsOperator(line.Substring(index, 2)))
                    {
                        token.Add(line.Substring(index, 2));
                        ++index;
                    }
                    else
                    {
                        token.Add(line.Substring(index, 1));
                    }
                    previndex = index + 1;
                }
            }
            ++index;
        }
        if (previndex != index)
        {
            temp = line.Substring(previndex, index - previndex).Trim();
            if (temp.Length != 0)
            {
                token.Add(temp);
            }
        }
        token.Add("");
        return token.ToArray();
    }

    public static string[] SplitStatement(string line)
    {
        var index = 0;
        var depth = 0;
        var previndex = 0;
        var length = line.Length;
        var isDoubleQuote = false;
        var stack = new Stack<int>();
        var statement = new List<string>();
        while (index < length)
        {
            if (line[index] == '\"')
            {
                if (index == 0 || line[index - 1] != '\\')
                {
                    isDoubleQuote = !isDoubleQuote;
                }
            }
            else if (!isDoubleQuote)
            {
                if (line[index] == '(')
                {
                    ++depth;
                }
                else if (line[index] == ')')
                {
                    --depth;
                }
                else if (depth == 0)
                {
                    if (line[index] == '{')
                    {
                        if (stack.Count == 0 && previndex != index)
                        {
                            statement.Add(line.Substring(previndex, index - previndex));
                        }
                        stack.Push(index);
                    }
                    else if (line[index] == '}')
                    {
                        var open = stack.Pop();
                        if (stack.Count == 0)
                        {
                            statement.Add(line.Substring(open, index - open + 1));
                            previndex = index + 1;
                        }
                    }
                    else if (line[index] == ';')
                    {
                        if (previndex != index)
                        {
                            statement.Add(line.Substring(previndex, index - previndex));
                        }
                        previndex = index + 1;
                    }
                }
            }
            ++index;
        }
        if (previndex != index)
        {
            statement.Add(line.Substring(previndex, index - previndex));
        }
        return statement.ToArray();
    }

    public static string[] SplitArrayIndexer(string line)
    {
        var index = 0;
        var depth = 0;
        var previndex = 0;
        var length = line.Length;
        var isDoubleQuote = false;
        var stack = new Stack<int>();
        var statement = new List<string>();
        while (index < length)
        {
            if (line[index] == '\"')
            {
                if (index == 0 || line[index - 1] != '\\')
                {
                    isDoubleQuote = !isDoubleQuote;
                }
            }
            else if (!isDoubleQuote)
            {
                if (line[index] == '{')
                {
                    ++depth;
                }
                else if (line[index] == '}')
                {
                    --depth;
                }
                else if (depth == 0)
                {
                    if (line[index] == '[')
                    {
                        if (stack.Count == 0 && previndex != index)
                        {
                            statement.Add(line.Substring(previndex, index - previndex));
                        }
                        stack.Push(index + 1);
                    }
                    else if (line[index] == ']')
                    {
                        var open = stack.Pop();
                        if (stack.Count == 0)
                        {
                            statement.Add(line.Substring(open, index - open));
                            previndex = index + 1;
                        }
                    }
                }
            }
            ++index;
        }
        if (previndex != index)
        {
            statement.Add(line.Substring(previndex, index - previndex));
        }
        return statement.ToArray();
    }

    public static string[] MakeFunctionStatement(string[] tokens)
    {
        var length = tokens.Length;
        var values = new List<string>();
        for (var i = 0; i < length; i++)
        {
            var value = tokens[i].Trim();
            if (value.Length != 0)
            {
                values.Add(value);
            }
        }
        return values.ToArray();
    }

    public static string[] MakeIfAndWhileStatement(string[] tokens)
    {
        var depth = 0;
        var index = 0;
        var previndex = 0;
        var length = tokens.Length;
        var value = new List<string>();
        for (index = 0; index < length; index++)
        {
            if (tokens[index] == "(")
            {
                if (depth == 0)
                {
                    if (previndex != index)
                    {
                        value.Add(string.Join("", tokens, previndex, index - previndex));
                    }
                    previndex = index;
                }
                ++depth;
            }
            else if (tokens[index] == ")")
            {
                --depth;
                if (depth == 0)
                {
                    value.Add(string.Join("", tokens, previndex + 1, index - previndex - 1));
                    previndex = index + 1;
                }
            }
            else if (IsKeyword(tokens[index]))
            {
                if (previndex != index)
                {
                    value.Add(string.Join("", tokens, previndex, index - previndex));
                }
                value.Add(string.Join("", tokens, previndex + 1, index - previndex));
                previndex = index + 1;
            }
        }
        if (previndex != index)
        {
            for (var i = previndex; i < tokens.Length; ++i)
            {
                value.Add(tokens[i]);
            }
        }
        return value.ToArray();
    }

    public static string[] MakeForAndForeachStatement(string[] tokens)
    {
        var depth = 0;
        var index = 0;
        var previndex = 0;
        var length = tokens.Length;
        var value = new List<string>();
        for (index = 0; index < length; index++)
        {
            if (tokens[index] == "(")
            {
                if (depth == 0)
                {
                    if (previndex != index)
                    {
                        value.Add(string.Join("", tokens, previndex, index - previndex));
                    }
                    previndex = index + 1;
                }
                ++depth;
            }
            else if (tokens[index] == ")")
            {
                --depth;
                if (depth == 0)
                {
                    value.Add(string.Join("", tokens, previndex, index - previndex));
                    previndex = index + 1;
                }
            }
            else if (depth == 1 && tokens[index] == ";")
            {
                if (previndex != index)
                {
                    value.Add(string.Join("", tokens, previndex, index - previndex));
                }
                previndex = index + 1;
            }
        }
        if (previndex != index)
        {
            for (var i = previndex; i < tokens.Length; ++i)
            {
                value.Add(tokens[i]);
            }
        }
        return value.ToArray();
    }

    public static string[]? MakeFunctionArguments(string[] tokens)
    {
        var length = tokens.Length;
        if (tokens[1] != "(")
        {
            // 関数呼び出しではない
            return null;
        }
        // 関数呼び出し演算子を探す
        var index = 1;
        var depth = 0;
        var previndex = index + 1;
        var arguments = new List<string>();
        while (index < length)
        {
            if (tokens[index] == "(")
            {
                ++depth;
            }
            else if (tokens[index] == ")")
            {
                --depth;
                if (depth == 0)
                {
                    break;
                }
            }
            else if (depth == 1 && tokens[index] == ",")
            {
                if (previndex != index)
                {
                    arguments.Add(string.Join("", tokens, previndex, index - previndex));
                }
                else
                {
                    arguments.Add("");
                }
                previndex = index + 1;
            }
            ++index;
        }
        if (previndex != index)
        {
            arguments.Add(string.Join("", tokens, previndex, index - previndex));
        }
        return arguments.ToArray();
    }
}
