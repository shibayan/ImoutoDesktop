using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ImoutoDesktop.MisakaSharp
{
    class Functions
    {
        public Functions()
        {
            functions = new SortedList<string, List<IFunction>>();
            systemFunctions = new SortedList<string, IFunction>();
            #region システム関数の登録
            // 基礎関数
            systemFunctions.Add("calc", new CalcFunction());
            systemFunctions.Add("random", new RandomFunction());
            systemFunctions.Add("parameter", new ParameterFunction());
            // 文字列関数
            systemFunctions.Add("extractfilename", new ExtractFileNameFunction());
            systemFunctions.Add("getvalue", new GetValueFunction());
            systemFunctions.Add("getvalueex", new GetValueExFunction());
            systemFunctions.Add("hiraganacase", new HiraganaCaseFunction());
            systemFunctions.Add("index", new IndexFunction());
            systemFunctions.Add("lastindex", new LastIndexFunction());
            systemFunctions.Add("insentence", new InSentenceFunction());
            systemFunctions.Add("isequallastandfirst", new IsEqualLastAndFirstFunction());
            systemFunctions.Add("length", new LengthFunction());
            systemFunctions.Add("substring", new SubStringFunction());
            systemFunctions.Add("substringfirst", new SubStringFirstFunction());
            systemFunctions.Add("substringl", new SubStringLeftFunction());
            systemFunctions.Add("substringlast", new SubStringLastFunction());
            systemFunctions.Add("substringr", new SubStringRightFunction());
            systemFunctions.Add("tolower", new ToLowerFunction());
            systemFunctions.Add("toupper", new ToUpperFunction());
            // 数学関数
            systemFunctions.Add("sin", new SinFunction());
            systemFunctions.Add("cos", new CosFunction());
            systemFunctions.Add("tan", new TanFunction());
            systemFunctions.Add("asin", new AsinFunction());
            systemFunctions.Add("acos", new AcosFunction());
            systemFunctions.Add("atan", new AtanFunction());
            systemFunctions.Add("sinh", new SinhFunction());
            systemFunctions.Add("cosh", new CoshFunction());
            systemFunctions.Add("tanh", new TanhFunction());
            systemFunctions.Add("abs", new AbsFunction());
            systemFunctions.Add("sqrt", new SqrtFunction());
            systemFunctions.Add("exp", new ExpFunction());
            systemFunctions.Add("log", new LogFunction());
            systemFunctions.Add("log10", new Log10Function());
            // 配列関数
            systemFunctions.Add("array", new ArrayFunction());
            systemFunctions.Add("count", new CountFunction());
            systemFunctions.Add("stringexists", new StringExistsFunction());
            // その他の関数
            systemFunctions.Add("backup", new BackupFunction());
            systemFunctions.Add("choice", new ChoiceFunction());
            systemFunctions.Add("search", new SearchFunction());
            #endregion
        }

        public void AddFunction(string name, IFunction function)
        {
            if (!functions.ContainsKey(name))
            {
                functions.Add(name, new List<IFunction>());
            }
            functions[name].Add(function);
        }

        public Value ExecFunction(MisakaVM vm, string name, params Value[] values)
        {
            // ローカル変数を作成
            LocalVariables lv = new LocalVariables();
            // 引数を作成する
            Value argv = new Value(values);
            lv.AddVariable("argv", argv);
            // 引数の数を保存する
            Value argc = new Value(values.Length);
            lv.AddVariable("argc", argc);
            // まずシステム関数を優先する
            if (systemFunctions.ContainsKey(name))
            {
                return systemFunctions[name].Execute(vm, lv);
            }
            // その後にユーザー登録関数から検索する
            List<IFunction> function = new List<IFunction>(functions[name]);
            for (int i = 0; i < function.Count; i++)
            {
                if (!function[i].IsExecutable(vm, lv))
                {
                    function.RemoveAt(i);
                    --i;
                }
            }
            if (function.Count != 0)
            {
                return function[vm.Random.Next(function.Count)].Execute(vm, lv);
            }
            else
            {
                return new Value();
            }
        }

        public Value SearchFunction(MisakaVM vm, params string[] patterns)
        {
            // ローカル変数を作成
            LocalVariables lv = new LocalVariables();
            // ユーザー登録関数から検索する
            IList<string> keys = functions.Keys;
            List<IFunction> function = new List<IFunction>();
            int length = keys.Count;
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < patterns.Length; j++)
                {
                    if (!Regex.IsMatch(keys[i], patterns[j]))
                    {
                    }
                }
            }
            int count = function.Count;
            for (int i = 0; i < count; i++)
            {
                if (!function[i].IsExecutable(vm, lv))
                {
                    function.RemoveAt(i--);
                    --count;
                }
            }
            if (count != 0)
            {
                return function[vm.Random.Next(count)].Execute(vm, lv);
            }
            else
            {
                return new Value();
            }
        }

        public bool IsFunction(string name)
        {
            return functions.ContainsKey(name) || systemFunctions.ContainsKey(name);
        }

        private SortedList<string, IFunction> systemFunctions;
        private SortedList<string, List<IFunction>> functions;
    }
}
