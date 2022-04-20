using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImoutoDesktop.Scripting;

/// <summary>
/// すべての関数のインターフェースです。
/// </summary>
internal interface IFunction
{
    /// <summary>
    /// 関数を実行します。
    /// </summary>
    /// <param name="vm">VMのインスタンス。</param>
    /// <param name="lv">ローカル変数管理オブジェクト。</param>
    /// <returns>関数の実行結果。</returns>
    Value Execute(ExecutionContext vm, LocalVariables lv);

    /// <summary>
    /// 関数が実行可能か調べます。
    /// </summary>
    /// <param name="vm">VMのインスタンス。</param>
    /// <param name="lv">ローカル変数管理オブジェクト。</param>
    /// <returns>実行可能かを示す値。</returns>
    bool IsExecutable(ExecutionContext vm, LocalVariables lv);
}

/// <summary>
/// ユーザ定義関数クラスです。
/// </summary>
internal class Function : IFunction
{
    /// <summary>
    /// 関数を初期化します。
    /// </summary>
    public Function()
    {
    }

    /// <summary>
    /// 関数を実行します。
    /// </summary>
    /// <param name="vm">VMのインスタンス。</param>
    /// <param name="lv">ローカル変数管理オブジェクト。</param>
    /// <returns>関数の実行結果。</returns>
    public Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var count = Statements.Count;
        for (var i = 0; i < count; i++)
        {
            _selector.Append(i);
        }
        var lines = _selector.Output(vm);
        if (lines == null)
        {
            return Value.Empty;
        }
        var length = lines.Length;
        if (length == 1)
        {
            var retval = ExecuteStatement(lines[0], vm, lv);
            return retval;
        }

        var result = new StringBuilder();
        for (var i = 0; i < length; i++)
        {
            var retval = ExecuteStatement(lines[i], vm, lv);
            if (retval != null && retval.ValueType != ValueType.Void)
            {
                result.Append(retval);
            }
        }
        return new Value(result.ToString());
    }

    /// <summary>
    /// 1つのステートメントを実行します。
    /// </summary>
    /// <param name="vm">VMのインスタンス。</param>
    /// <param name="lv">ローカル変数管理オブジェクト。</param>
    /// <returns>ステートメントの実行結果。</returns>
    private Value ExecuteStatement(int line, ExecutionContext vm, LocalVariables lv)
    {
        var length = Statements[line].Length;
        if (length == 1)
        {
            // ステートメントがひとつなら型は保護される
            var retval = Statements[line][0].Evaluate(vm, lv);
            return retval;
        }

        var result = new StringBuilder();
        for (var i = 0; i < length; i++)
        {
            var retval = Statements[line][i].Evaluate(vm, lv);
            if (retval != null && retval.ValueType != ValueType.Void)
            {
                result.Append(retval);
            }
        }
        return new Value(result.ToString());
    }

    /// <summary>
    /// 関数が実行可能か調べます。
    /// </summary>
    /// <param name="vm">VMのインスタンス。</param>
    /// <param name="lv">ローカル変数管理オブジェクト。</param>
    /// <returns>実行可能かを示す値。</returns>
    public bool IsExecutable(ExecutionContext vm, LocalVariables lv)
    {
        var length = Expressions.Count;
        for (var i = 0; i < length; ++i)
        {
            // 採用条件式を評価する
            var value = Expressions[i].Evaluate(vm, lv);
            if (!value.ToBoolean())
            {
                // 1つでもfalseがあると実行できない
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// この関数が含まれている辞書のファイル名。
    /// </summary>
    public string DictionaryName { get; set; }

    /// <summary>
    /// この関数の辞書内での位置。
    /// </summary>
    public int DictionaryLine { get; set; }

    /// <summary>
    /// この関数の採用条件式。
    /// </summary>
    internal List<IExpression> Expressions { get; } = new();

    /// <summary>
    /// この関数のステートメント。
    /// </summary>
    internal List<IExpression[]> Statements { get; } = new();

    private readonly Selector _selector = new();

    /// <summary>
    /// 出力選択のモードを取得、設定します。
    /// </summary>
    internal SelectType SelectType
    {
        get => _selector.SelectType;
        set => _selector.SelectType = value;
    }
}

/// <summary>
/// システム関数の基底クラス。
/// </summary>
internal class SystemFunction : IFunction
{
    public virtual Value Execute(ExecutionContext vm, LocalVariables lv) => Value.Empty;

    // システム関数は常に実行可能
    public bool IsExecutable(ExecutionContext vm, LocalVariables lv) => true;
}

internal class AdjustPrefix : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        return base.Execute(vm, lv);
    }
}

internal class BackupFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        // 変数のシリアライズ
        vm.Variables.Serialize(Path.Combine(vm.RootDirectory, "imouto_vars.txt"));
        return Value.Empty;
    }
}

internal class CalcFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return argv[0].Clone();
    }
}

internal class ChoiceFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        var index = vm.Random.Next(argv.Count);
        return argv[index];
    }
}

internal class CountFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(argv[0].Count);
    }
}

internal class ExtractFileNameFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(Path.GetFileName(argv[0].ToString()));
    }
}

internal class GetValueFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        var index = argv[1].ToInt32();
        var tokens = argv[0].ToString().Split(',');
        if (tokens.Length > index)
        {
            return new Value(tokens[index]);
        }
        return Value.Empty;
    }
}

internal class GetValueExFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        var index = argv[1].ToInt32();
        var tokens = argv[0].ToString().Split('\x1');
        if (tokens.Length > index)
        {
            return new Value(tokens[index]);
        }
        return Value.Empty;
    }
}

internal class HiraganaCaseFunction : SystemFunction
{
}

internal class IndexFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(argv[1].ToString().IndexOf(argv[0].ToString()));
    }
}

internal class LastIndexFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(argv[1].ToString().LastIndexOf(argv[0].ToString()));
    }
}

internal class InSentenceFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        var text = argv[0].ToString();
        for (var i = 1; i < argv.Count; i++)
        {
            if (text.IndexOf(argv[i].ToString()) == -1)
            {
                return new Value(false);
            }
        }
        return new Value(true);
    }
}

internal class IsEqualLastAndFirstFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        var first = argv[0].ToString();
        var second = argv[1].ToString();
        return new Value(first[^1] == second[0]);
    }
}

internal class LengthFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(argv[0].ToString().Length);
    }
}

internal class RandomFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(vm.Random.Next(argv[0].ToInt32()));
    }
}

internal class ParameterFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        var index = argv[0].ToInt32();
        if (vm.Parameter != null && vm.Parameter.Length > index)
        {
            return new Value(vm.Parameter[index]);
        }
        return Value.Empty;
    }
}

internal class SearchFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return base.Execute(vm, lv);
    }
}

internal class StringExistsFunction : SystemFunction
{
}

internal class SubStringFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        var str = argv[0].ToString();
        var offset = argv[1].ToInt32();
        var count = argv[2].ToInt32();
        return new Value(str.Substring(offset, count));
    }
}

internal class SubStringFirstFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        var str = argv[0].ToString();
        return new Value(str.Substring(0, 1));
    }
}

internal class SubStringLeftFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        var str = argv[0].ToString();
        var count = argv[1].ToInt32();
        return new Value(str.Substring(0, count));
    }
}

internal class SubStringLastFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        var str = argv[0].ToString();
        return new Value(str.Substring(str.Length - 1, 1));
    }
}

internal class SubStringRightFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        var str = argv[0].ToString();
        var count = argv[1].ToInt32();
        return new Value(str.Substring(str.Length - count, count));
    }
}

internal class SinFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(Math.Sin(argv[0].ToDouble()));
    }
}

internal class CosFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(Math.Cos(argv[0].ToDouble()));
    }
}

internal class TanFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(Math.Tan(argv[0].ToDouble()));
    }
}

internal class AsinFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(Math.Asin(argv[0].ToDouble()));
    }
}

internal class AcosFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(Math.Acos(argv[0].ToDouble()));
    }
}

internal class AtanFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(Math.Atan(argv[0].ToDouble()));
    }
}

internal class SinhFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(Math.Sinh(argv[0].ToDouble()));
    }
}

internal class CoshFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(Math.Cosh(argv[0].ToDouble()));
    }
}

internal class TanhFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(Math.Tanh(argv[0].ToDouble()));
    }
}

internal class AbsFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(Math.Abs(argv[0].ToInt32()));
    }
}

internal class SqrtFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(Math.Sqrt(argv[0].ToDouble()));
    }
}

internal class ExpFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(Math.Exp(argv[0].ToDouble()));
    }
}

internal class LogFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(Math.Log(argv[0].ToDouble()));
    }
}

internal class Log10Function : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(Math.Log10(argv[0].ToDouble()));
    }
}

internal class ArrayFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(argv.ToArray());
    }
}

internal class ToLowerFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(argv[0].ToString().ToLowerInvariant());
    }
}

internal class ToUpperFunction : SystemFunction
{
    public override Value Execute(ExecutionContext vm, LocalVariables lv)
    {
        var argv = lv.GetVariable("argv");
        return new Value(argv[0].ToString().ToUpperInvariant());
    }
}
