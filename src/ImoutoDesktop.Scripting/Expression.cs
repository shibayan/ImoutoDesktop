using System;
using System.Collections.Generic;
using System.Text;

namespace ImoutoDesktop.Scripting;

internal interface IExpression
{
    Value Evaluate(ExecutionContext vm, LocalVariables lv);
}

internal class UnaryExpression : IExpression
{
    public UnaryExpression(IExpression expression)
    {
        Expression = expression;
    }

    public virtual Value Evaluate(ExecutionContext vm, LocalVariables lv) => null;

    protected IExpression Expression { get; init; }
}

internal class UnaryAddExpression : UnaryExpression
{
    public UnaryAddExpression(IExpression expression)
        : base(expression)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv) => Expression.Evaluate(vm, lv);
}

internal class UnarySubExpression : UnaryExpression
{
    public UnarySubExpression(IExpression expression)
        : base(expression)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var value = Expression.Evaluate(vm, lv);
        if (value.ValueType == ValueType.Double)
        {
            return new Value(-value.ToDouble());
        }
        return new Value(-value.ToInt32());
    }
}

internal class UnaryNotExpression : UnaryExpression
{
    public UnaryNotExpression(IExpression expression)
        : base(expression)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var value = Expression.Evaluate(vm, lv);
        return new Value(!value.ToBoolean());
    }
}

internal class UnaryCompExpression : UnaryExpression
{
    public UnaryCompExpression(IExpression expression)
        : base(expression)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var value = Expression.Evaluate(vm, lv);
        return new Value(~value.ToInt32());
    }
}

internal class BinaryExpression : IExpression
{
    public BinaryExpression(IExpression lhs, IExpression rhs)
    {
        LeftExpression = lhs;
        RightExpression = rhs;
    }

    public virtual Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        return null;
    }

    public virtual bool IsOutputable => true;

    protected IExpression LeftExpression { get; init; }
    protected IExpression RightExpression { get; init; }
}

internal class AddExpression : BinaryExpression
{
    public AddExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        var valueType = lhs.GetTypePromotion(rhs);
        switch (valueType)
        {
            case ValueType.Int32:
                return new Value(lhs.ToInt32() + rhs.ToInt32());
            case ValueType.Double:
                return new Value(lhs.ToDouble() + rhs.ToDouble());
            case ValueType.String:
                return new Value(lhs + rhs.ToString());
            case ValueType.Void:
            case ValueType.Boolean:
            case ValueType.Array:
            default:
                return new Value(lhs.ToInt32() + rhs.ToInt32());
        }
    }
}

internal class SubExpression : BinaryExpression
{
    public SubExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        var valueType = lhs.GetTypePromotion(rhs);
        if (valueType == ValueType.Double)
        {
            return new Value(lhs.ToDouble() - rhs.ToDouble());
        }
        return new Value(lhs.ToInt32() - rhs.ToInt32());
    }
}

internal class MulExpression : BinaryExpression
{
    public MulExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        var valueType = lhs.GetTypePromotion(rhs);
        if (valueType == ValueType.Double)
        {
            return new Value(lhs.ToDouble() * rhs.ToDouble());
        }
        return new Value(lhs.ToInt32() * rhs.ToInt32());
    }
}

internal class DivExpression : BinaryExpression
{
    public DivExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        var valueType = lhs.GetTypePromotion(rhs);
        if (valueType == ValueType.Double)
        {
            var dValue = rhs.ToDouble();
            if (dValue != 0.0)
            {
                return new Value(lhs.ToDouble() / rhs.ToDouble());
            }
        }
        else
        {
            var iValue = rhs.ToInt32();
            if (iValue != 0)
            {
                return new Value(lhs.ToInt32() / rhs.ToInt32());
            }
        }
        return Value.Empty;
    }
}

internal class ModExpression : BinaryExpression
{
    public ModExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        var valueType = lhs.GetTypePromotion(rhs);
        if (valueType == ValueType.Double)
        {
            return new Value(lhs.ToDouble() % rhs.ToDouble());
        }
        return new Value(lhs.ToInt32() % rhs.ToInt32());
    }
}

internal class PowExpression : BinaryExpression
{
    public PowExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        var valueType = lhs.GetTypePromotion(rhs);
        if (valueType == ValueType.Double)
        {
            return new Value(Math.Pow(lhs.ToDouble(), rhs.ToDouble()));
        }
        return new Value((int)Math.Pow(lhs.ToInt32(), rhs.ToInt32()));
    }
}

internal class EqualExpression : BinaryExpression
{
    public EqualExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        var valueType = lhs.GetTypePromotion(rhs);
        switch (valueType)
        {
            case ValueType.Void:
                return new Value(true);
            case ValueType.Boolean:
                return new Value(lhs.ToBoolean() == rhs.ToBoolean());
            case ValueType.Int32:
                return new Value(lhs.ToInt32() == rhs.ToInt32());
            case ValueType.Double:
                return new Value(lhs.ToDouble() == rhs.ToDouble());
            case ValueType.String:
                return new Value(lhs.ToString() == rhs.ToString());
            default:
                return new Value(false);
        }
    }
}

internal class NotEqualExpression : BinaryExpression
{
    public NotEqualExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        var valueType = lhs.GetTypePromotion(rhs);
        switch (valueType)
        {
            case ValueType.Void:
                return new Value(false);
            case ValueType.Boolean:
                return new Value(lhs.ToBoolean() != rhs.ToBoolean());
            case ValueType.Int32:
                return new Value(lhs.ToInt32() != rhs.ToInt32());
            case ValueType.Double:
                return new Value(lhs.ToDouble() != rhs.ToDouble());
            case ValueType.String:
                return new Value(lhs.ToString() != rhs.ToString());
            default:
                return new Value(true);
        }
    }
}

internal class LessExpression : BinaryExpression
{
    public LessExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        var valueType = lhs.GetTypePromotion(rhs);
        switch (valueType)
        {
            case ValueType.Int32:
                return new Value(lhs.ToInt32() < rhs.ToInt32());
            case ValueType.Double:
                return new Value(lhs.ToDouble() < rhs.ToDouble());
            case ValueType.String:
                return new Value(Comparer<string>.Default.Compare(lhs.ToString(), rhs.ToString()) < 0);
            case ValueType.Void:
            case ValueType.Boolean:
            case ValueType.Array:
            default:
                return new Value(lhs.ToInt32() < rhs.ToInt32());
        }
    }
}

internal class LessEqualExpression : BinaryExpression
{
    public LessEqualExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        var valueType = lhs.GetTypePromotion(rhs);
        switch (valueType)
        {
            case ValueType.Int32:
                return new Value(lhs.ToInt32() <= rhs.ToInt32());
            case ValueType.Double:
                return new Value(lhs.ToDouble() <= rhs.ToDouble());
            case ValueType.String:
                return new Value(Comparer<string>.Default.Compare(lhs.ToString(), rhs.ToString()) <= 0);
            default:
                return new Value(lhs.ToInt32() <= rhs.ToInt32());
        }
    }
}

internal class GreaterExpression : BinaryExpression
{
    public GreaterExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        var valueType = lhs.GetTypePromotion(rhs);
        switch (valueType)
        {
            case ValueType.Int32:
                return new Value(lhs.ToInt32() > rhs.ToInt32());
            case ValueType.Double:
                return new Value(lhs.ToDouble() > rhs.ToDouble());
            case ValueType.String:
                return new Value(Comparer<string>.Default.Compare(lhs.ToString(), rhs.ToString()) > 0);
            default:
                return new Value(lhs.ToInt32() > rhs.ToInt32());
        }
    }
}

internal class GreaterEqualExpression : BinaryExpression
{
    public GreaterEqualExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        var valueType = lhs.GetTypePromotion(rhs);
        switch (valueType)
        {
            case ValueType.Int32:
                return new Value(lhs.ToInt32() >= rhs.ToInt32());
            case ValueType.Double:
                return new Value(lhs.ToDouble() >= rhs.ToDouble());
            case ValueType.String:
                return new Value(Comparer<string>.Default.Compare(lhs.ToString(), rhs.ToString()) >= 0);
            default:
                return new Value(lhs.ToInt32() >= rhs.ToInt32());
        }
    }
}

internal class LogicalOrExpression : BinaryExpression
{
    public LogicalOrExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        if (lhs.ToBoolean())
        {
            return new Value(true);
        }
        var rhs = RightExpression.Evaluate(vm, lv);
        return new Value(rhs.ToBoolean());
    }
}

internal class LogicalAndExpression : BinaryExpression
{
    public LogicalAndExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        if (!lhs.ToBoolean())
        {
            return new Value(false);
        }
        var rhs = RightExpression.Evaluate(vm, lv);
        return !rhs.ToBoolean() ? new Value(false) : new Value(true);
    }
}

internal class SubstituteExpression : BinaryExpression
{
    public SubstituteExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        rhs.CopyTo(lhs);
        return Value.Empty;
    }
}

internal class SubstituteAddExpression : BinaryExpression
{
    public SubstituteAddExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        Value value;
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        var valueType = lhs.GetTypePromotion(rhs);
        switch (valueType)
        {
            case ValueType.Int32:
                value = new Value(lhs.ToInt32() + rhs.ToInt32());
                break;
            case ValueType.Double:
                value = new Value(lhs.ToDouble() + rhs.ToDouble());
                break;
            case ValueType.String:
                value = new Value(lhs + rhs.ToString());
                break;
            default:
                value = new Value(lhs.ToInt32() + rhs.ToInt32());
                break;
        }
        value.CopyTo(lhs);
        return Value.Empty;
    }
}

internal class SubstituteSubExpression : BinaryExpression
{
    public SubstituteSubExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        Value value;
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        var valueType = lhs.GetTypePromotion(rhs);
        if (valueType == ValueType.Double)
        {
            value = new Value(lhs.ToDouble() - rhs.ToDouble());
        }
        else
        {
            value = new Value(lhs.ToInt32() - rhs.ToInt32());
        }
        value.CopyTo(lhs);
        return Value.Empty;
    }

    public override bool IsOutputable => false;
}

internal class SubstituteMulExpression : BinaryExpression
{
    public SubstituteMulExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        Value value;
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        var valueType = lhs.GetTypePromotion(rhs);
        if (valueType == ValueType.Double)
        {
            value = new Value(lhs.ToDouble() * rhs.ToDouble());
        }
        else
        {
            value = new Value(lhs.ToInt32() * rhs.ToInt32());
        }
        value.CopyTo(lhs);
        return Value.Empty;
    }
}

internal class SubstituteDivExpression : BinaryExpression
{
    public SubstituteDivExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        Value value;
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        var valueType = lhs.GetTypePromotion(rhs);
        if (valueType == ValueType.Double)
        {
            var dValue = rhs.ToDouble();
            if (dValue != 0.0)
            {
                value = new Value(lhs.ToDouble() / rhs.ToDouble());
                value.CopyTo(lhs);
            }
        }
        else
        {
            var iValue = rhs.ToInt32();
            if (iValue != 0)
            {
                value = new Value(lhs.ToInt32() / rhs.ToInt32());
                value.CopyTo(lhs);
            }
        }
        return Value.Empty;
    }
}

internal class ShiftLeftExpression : BinaryExpression
{
    public ShiftLeftExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        return new Value(lhs.ToInt32() << rhs.ToInt32());
    }
}

internal class ShiftRightExpression : BinaryExpression
{
    public ShiftRightExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        return new Value(lhs.ToInt32() >> rhs.ToInt32());
    }
}

internal class BitwiseOrExpression : BinaryExpression
{
    public BitwiseOrExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        return new Value(lhs.ToInt32() | rhs.ToInt32());
    }
}

internal class BitwiseAndExpression : BinaryExpression
{
    public BitwiseAndExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        return new Value(lhs.ToInt32() & rhs.ToInt32());
    }
}

internal class BitwiseXorExpression : BinaryExpression
{
    public BitwiseXorExpression(IExpression lhs, IExpression rhs)
        : base(lhs, rhs)
    {
    }

    public override Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var lhs = LeftExpression.Evaluate(vm, lv);
        var rhs = RightExpression.Evaluate(vm, lv);
        return new Value(lhs.ToInt32() ^ rhs.ToInt32());
    }
}

internal class ValueExpression : IExpression
{
    public ValueExpression(Value value)
    {
        _value = value;
    }

    public Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        if (_value.ValueType == ValueType.String)
        {
            var statements = Lexer.SplitStatement(_value.ToString());
            var length = statements.Length;
            var result = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                if (Lexer.IsEvaluate(statements[i]))
                {
                    var expression = vm.Parser.MakeExpression(statements[i]);
                    result.Append(expression.Evaluate(vm, lv));
                }
                else
                {
                    result.Append(statements[i]);
                }
            }
            return new Value(result.ToString());
        }
        return _value;
    }

    private readonly Value _value;
}

internal class EvaluateExpression : IExpression
{
    public EvaluateExpression(string statement)
    {
        _statements = Lexer.SplitStatement(statement);
    }

    public Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var length = _statements.Length;
        var result = new StringBuilder();
        for (var i = 0; i < length; i++)
        {
            if (Lexer.IsEvaluate(_statements[i]))
            {
                var expression = vm.Parser.MakeExpression(_statements[i]);
                result.Append(expression.Evaluate(vm, lv));
            }
            else
            {
                result.Append(_statements[i]);
            }
        }
        int index;
        Value value;
        var name = result.ToString();
        var array = Lexer.SplitArrayIndexer(name);
        var statement = array[0].Substring(1);
        if (LocalVariables.IsLocalVariable(name))
        {
            if (array.Length > 1)
            {
                index = int.Parse(array[1]);
                value = lv.GetVariable(statement)[index].Clone();
            }
            else
            {
                value = lv.GetVariable(statement).Clone();
            }
        }
        else
        {
            if (array.Length > 1)
            {
                index = int.Parse(array[1]);
                if (vm.Functions.IsFunction(statement))
                {
                    return vm.Functions.ExecFunction(vm, statement)[index];
                }
                value = vm.Variables.GetVariable(statement)[index].Clone();
            }
            else
            {
                if (vm.Functions.IsFunction(statement))
                {
                    return vm.Functions.ExecFunction(vm, statement);
                }
                value = vm.Variables.GetVariable(statement).Clone();
            }
        }
        if (value.ValueType == ValueType.Array)
        {
            index = vm.Random.Next(value.ToArray().Count);
            return value[index].Clone();
        }
        return value;
    }

    private readonly string[] _statements;
}

internal class VariableExpression : IExpression
{
    public VariableExpression(string statement)
    {
        _statements = Lexer.SplitStatement(statement);
    }

    public Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var length = _statements.Length;
        var result = new StringBuilder();
        for (var i = 0; i < length; i++)
        {
            if (Lexer.IsEvaluate(_statements[i]))
            {
                var expression = vm.Parser.MakeExpression(_statements[i]);
                result.Append(expression.Evaluate(vm, lv));
            }
            else
            {
                result.Append(_statements[i]);
            }
        }
        var index = 0;
        var name = result.ToString();
        var array = Lexer.SplitArrayIndexer(name);
        var statement = array[0].Substring(1);
        if (LocalVariables.IsLocalVariable(name))
        {
            if (array.Length > 1)
            {
                index = int.Parse(array[1]);
                return lv.GetVariable(statement)[index];
            }
            return lv.GetVariable(statement);
        }
        if (array.Length > 1)
        {
            index = int.Parse(array[1]);
            if (vm.Functions.IsFunction(statement))
            {
                return vm.Functions.ExecFunction(vm, statement)[index];
            }

            return vm.Variables.GetVariable(statement)[index];
        }

        if (vm.Functions.IsFunction(statement))
        {
            return vm.Functions.ExecFunction(vm, statement);
        }

        return vm.Variables.GetVariable(statement);
    }

    private readonly string[] _statements;
}

internal class FunctionExpression : IExpression
{
    public FunctionExpression(string statement, IExpression[] expressions)
    {
        _expressions = expressions;
        _statements = Lexer.SplitStatement(statement.Substring(1));
    }

    public Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var length = _statements.Length;
        var result = new StringBuilder();
        for (var i = 0; i < length; i++)
        {
            if (Lexer.IsEvaluate(_statements[i]))
            {
                var expression = vm.Parser.MakeExpression(_statements[i]);
                result.Append(expression.Evaluate(vm, lv));
            }
            else
            {
                result.Append(_statements[i]);
            }
        }
        // 関数名を確定する
        var statement = result.ToString();
        // 引数を評価する
        length = _expressions.Length;
        var values = new Value[length];
        for (var i = 0; i < length; i++)
        {
            values[i] = _expressions[i].Evaluate(vm, lv);
        }
        if (vm.Functions.IsFunction(statement))
        {
            return vm.Functions.ExecFunction(vm, statement, values);
        }
        return Value.Empty;
    }

    private readonly string[] _statements;
    private readonly IExpression[] _expressions;
}

internal class IfExpression : IExpression
{
    public IfExpression(IfSubExpression[] expressions)
    {
        _expressions = expressions;
    }

    public Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var length = _expressions.Length;
        if (length == 1 && !_expressions[0].IsHaveStatements())
        {
            // 処理部省略のとき
            return new Value(_expressions[0].IsExecutable(vm, lv));
        }
        for (var i = 0; i < length; i++)
        {
            if (_expressions[i].IsExecutable(vm, lv))
            {
                return _expressions[i].Evaluate(vm, lv);
            }
        }
        return Value.Empty;
    }

    private readonly IfSubExpression[] _expressions;
}

internal class IfSubExpression : IExpression
{
    public IfSubExpression(IExpression expression, IExpression[] statements)
    {
        _expression = expression;
        _statements = statements;
    }

    public Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var length = _statements.Length;
        if (length == 1)
        {
            return _statements[0].Evaluate(vm, lv);
        }
        var result = new StringBuilder();
        for (var i = 0; i < length; i++)
        {
            var retval = _statements[i].Evaluate(vm, lv);
            if (retval != null && retval.ValueType != ValueType.Void)
            {
                result.Append(retval);
            }
        }
        return new Value(result.ToString());
    }

    public bool IsHaveStatements()
    {
        return _statements.Length != 0;
    }

    public bool IsExecutable(ExecutionContext vm, LocalVariables lv)
    {
        return _expression.Evaluate(vm, lv).ToBoolean();
    }

    private readonly IExpression _expression;
    private readonly IExpression[] _statements;
}

internal class WhileExpression : IExpression
{
    public WhileExpression(IExpression expression, IExpression[] statements)
    {
        _expression = expression;
        _statements = statements;
    }

    public Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var length = _statements.Length;
        var result = new StringBuilder();
        while (_expression.Evaluate(vm, lv).ToBoolean())
        {
            for (var i = 0; i < length; ++i)
            {
                var retval = _statements[i].Evaluate(vm, lv);
                if (retval != null && retval.ValueType != ValueType.Void)
                {
                    result.Append(retval);
                }
            }
        }
        return new Value(result.ToString());
    }

    private readonly IExpression _expression;
    private readonly IExpression[] _statements;
}

internal class ForExpression : IExpression
{
    public ForExpression(IExpression[] expressions, IExpression[] statements)
    {
        _expressions = expressions;
        _statements = statements;
    }

    public Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var length = _statements.Length;
        var result = new StringBuilder();
        for (_expressions[0].Evaluate(vm, lv); _expressions[1].Evaluate(vm, lv).ToBoolean(); _expressions[2].Evaluate(vm, lv))
        {
            for (var i = 0; i < length; i++)
            {
                var retval = _statements[i].Evaluate(vm, lv);
                if (retval != null && retval.ValueType != ValueType.Void)
                {
                    result.Append(retval);
                }
            }
        }
        return new Value(result.ToString());
    }

    private readonly IExpression[] _expressions;
    private readonly IExpression[] _statements;
}

internal class ForeachExpression : IExpression
{
    public ForeachExpression(IExpression[] expressions, IExpression[] statements)
    {
        _expressions = expressions;
        _statements = statements;
    }

    public Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        var var = _expressions[0].Evaluate(vm, lv);
        var array = _expressions[1].Evaluate(vm, lv).ToArray();
        var length = _statements.Length;
        var result = new StringBuilder();
        foreach (var temp in array)
        {
            temp.CopyTo(var);
            for (var i = 0; i < length; i++)
            {
                var retval = _statements[i].Evaluate(vm, lv);
                if (retval != null && retval.ValueType != ValueType.Void)
                {
                    result.Append(retval);
                }
            }
        }
        return new Value(result.ToString());
    }

    private readonly IExpression[] _expressions;
    private readonly IExpression[] _statements;
}

internal class TextExpression : IExpression
{
    public TextExpression(string text)
    {
        _text = text;
    }

    public Value Evaluate(ExecutionContext vm, LocalVariables lv)
    {
        return new Value(_text);
    }

    private readonly string _text;
}
