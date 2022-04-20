using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ImoutoDesktop.Scripting;

internal class Functions
{
    public Functions()
    {
        #region システム関数の登録
        // 基礎関数
        _systemFunctions.Add("calc", new CalcFunction());
        _systemFunctions.Add("random", new RandomFunction());
        _systemFunctions.Add("parameter", new ParameterFunction());
        // 文字列関数
        _systemFunctions.Add("extractfilename", new ExtractFileNameFunction());
        _systemFunctions.Add("getvalue", new GetValueFunction());
        _systemFunctions.Add("getvalueex", new GetValueExFunction());
        _systemFunctions.Add("hiraganacase", new HiraganaCaseFunction());
        _systemFunctions.Add("index", new IndexFunction());
        _systemFunctions.Add("lastindex", new LastIndexFunction());
        _systemFunctions.Add("insentence", new InSentenceFunction());
        _systemFunctions.Add("isequallastandfirst", new IsEqualLastAndFirstFunction());
        _systemFunctions.Add("length", new LengthFunction());
        _systemFunctions.Add("substring", new SubStringFunction());
        _systemFunctions.Add("substringfirst", new SubStringFirstFunction());
        _systemFunctions.Add("substringl", new SubStringLeftFunction());
        _systemFunctions.Add("substringlast", new SubStringLastFunction());
        _systemFunctions.Add("substringr", new SubStringRightFunction());
        _systemFunctions.Add("tolower", new ToLowerFunction());
        _systemFunctions.Add("toupper", new ToUpperFunction());
        // 数学関数
        _systemFunctions.Add("sin", new SinFunction());
        _systemFunctions.Add("cos", new CosFunction());
        _systemFunctions.Add("tan", new TanFunction());
        _systemFunctions.Add("asin", new AsinFunction());
        _systemFunctions.Add("acos", new AcosFunction());
        _systemFunctions.Add("atan", new AtanFunction());
        _systemFunctions.Add("sinh", new SinhFunction());
        _systemFunctions.Add("cosh", new CoshFunction());
        _systemFunctions.Add("tanh", new TanhFunction());
        _systemFunctions.Add("abs", new AbsFunction());
        _systemFunctions.Add("sqrt", new SqrtFunction());
        _systemFunctions.Add("exp", new ExpFunction());
        _systemFunctions.Add("log", new LogFunction());
        _systemFunctions.Add("log10", new Log10Function());
        // 配列関数
        _systemFunctions.Add("array", new ArrayFunction());
        _systemFunctions.Add("count", new CountFunction());
        _systemFunctions.Add("stringexists", new StringExistsFunction());
        // その他の関数
        _systemFunctions.Add("backup", new BackupFunction());
        _systemFunctions.Add("choice", new ChoiceFunction());
        _systemFunctions.Add("search", new SearchFunction());
        #endregion
    }

    public void AddFunction(string name, IFunction function)
    {
        if (!_functions.ContainsKey(name))
        {
            _functions.Add(name, new List<IFunction>());
        }
        _functions[name].Add(function);
    }

    public Value ExecFunction(ExecutionContext vm, string name, params Value[] values)
    {
        // ローカル変数を作成
        var lv = new LocalVariables();
        // 引数を作成する
        var argv = new Value(values);
        lv.AddVariable("argv", argv);
        // 引数の数を保存する
        var argc = new Value(values.Length);
        lv.AddVariable("argc", argc);
        // まずシステム関数を優先する
        if (_systemFunctions.ContainsKey(name))
        {
            return _systemFunctions[name].Execute(vm, lv);
        }
        // その後にユーザー登録関数から検索する
        var function = new List<IFunction>(_functions[name]);
        for (var i = 0; i < function.Count; i++)
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

        return Value.Empty;
    }

    public Value SearchFunction(ExecutionContext vm, params string[] patterns)
    {
        // ローカル変数を作成
        var lv = new LocalVariables();
        // ユーザー登録関数から検索する
        var keys = _functions.Keys;
        var function = new List<IFunction>();
        var length = keys.Count;
        for (var i = 0; i < length; i++)
        {
            for (var j = 0; j < patterns.Length; j++)
            {
                if (!Regex.IsMatch(keys[i], patterns[j]))
                {
                }
            }
        }
        var count = function.Count;
        for (var i = 0; i < count; i++)
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

        return Value.Empty;
    }

    public bool IsFunction(string name) => _functions.ContainsKey(name) || _systemFunctions.ContainsKey(name);

    private readonly SortedList<string, IFunction> _systemFunctions = new();
    private readonly SortedList<string, List<IFunction>> _functions = new();
}
