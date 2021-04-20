using System;
using System.Collections.Generic;
using System.Text;

namespace ImoutoDesktop.MisakaSharp
{
    interface IExpression
    {
        Value Evaluate(MisakaVM vm, LocalVariables lv);
    }

    class UnaryExpression : IExpression
    {
        public UnaryExpression(IExpression expression)
        {
            this.expression = expression;
        }

        public virtual Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            return null;
        }

        protected IExpression expression;
    }

    class UnaryAddExpression : UnaryExpression
    {
        public UnaryAddExpression(IExpression expression)
            : base(expression)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            return expression.Evaluate(vm, lv);
        }
    }

    class UnarySubExpression : UnaryExpression
    {
        public UnarySubExpression(IExpression expression)
            : base(expression)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value value = expression.Evaluate(vm, lv);
            if (value.ValueType == ValueType.Double)
            {
                return new Value(-value.ToDouble());
            }
            return new Value(-value.ToInt32());
        }
    }

    class UnaryNotExpression : UnaryExpression
    {
        public UnaryNotExpression(IExpression expression)
            : base(expression)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value value = expression.Evaluate(vm, lv);
            return new Value(!value.ToBoolean());
        }
    }

    class UnaryCompExpression : UnaryExpression
    {
        public UnaryCompExpression(IExpression expression)
            : base(expression)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value value = expression.Evaluate(vm, lv);
            return new Value(~value.ToInt32());
        }
    }

    class BinaryExpression : IExpression
    {
        public BinaryExpression(IExpression lhs, IExpression rhs)
        {
            this.leftExpression = lhs;
            this.rightExpression = rhs;
        }

        public virtual Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            return null;
        }

        public virtual bool IsOutputable
        {
            get
            {
                return true;
            }
        }

        protected IExpression leftExpression;
        protected IExpression rightExpression;
    }

    class AddExpression : BinaryExpression
    {
        public AddExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            ValueType valueType = lhs.GetTypePromotion(rhs);
            switch (valueType)
            {
                case ValueType.Int32:
                    return new Value(lhs.ToInt32() + rhs.ToInt32());
                case ValueType.Double:
                    return new Value(lhs.ToDouble() + rhs.ToDouble());
                case ValueType.String:
                    return new Value(lhs.ToString() + rhs.ToString());
                case ValueType.Void:
                case ValueType.Boolean:
                case ValueType.Array:
                default:
                    return new Value(lhs.ToInt32() + rhs.ToInt32());
            }
        }
    }

    class SubExpression : BinaryExpression
    {
        public SubExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            ValueType valueType = lhs.GetTypePromotion(rhs);
            if (valueType == ValueType.Double)
            {
                return new Value(lhs.ToDouble() - rhs.ToDouble());
            }
            return new Value(lhs.ToInt32() - rhs.ToInt32());
        }
    }

    class MulExpression : BinaryExpression
    {
        public MulExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            ValueType valueType = lhs.GetTypePromotion(rhs);
            if (valueType == ValueType.Double)
            {
                return new Value(lhs.ToDouble() * rhs.ToDouble());
            }
            return new Value(lhs.ToInt32() * rhs.ToInt32());
        }
    }

    class DivExpression : BinaryExpression
    {
        public DivExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            ValueType valueType = lhs.GetTypePromotion(rhs);
            if (valueType == ValueType.Double)
            {
                double d_value = rhs.ToDouble();
                if (d_value != 0.0)
                {
                    return new Value(lhs.ToDouble() / rhs.ToDouble());
                }
            }
            else
            {
                int i_value = rhs.ToInt32();
                if (i_value != 0)
                {
                    return new Value(lhs.ToInt32() / rhs.ToInt32());
                }
            }
            return new Value();
        }
    }

    class ModExpression : BinaryExpression
    {
        public ModExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            ValueType valueType = lhs.GetTypePromotion(rhs);
            if (valueType == ValueType.Double)
            {
                return new Value(lhs.ToDouble() % rhs.ToDouble());
            }
            return new Value(lhs.ToInt32() % rhs.ToInt32());
        }
    }

    class PowExpression : BinaryExpression
    {
        public PowExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            ValueType valueType = lhs.GetTypePromotion(rhs);
            if (valueType == ValueType.Double)
            {
                return new Value(Math.Pow(lhs.ToDouble(), rhs.ToDouble()));
            }
            return new Value((int)Math.Pow(lhs.ToInt32(),rhs.ToInt32()));
        }
    }

    class EqualExpression : BinaryExpression
    {
        public EqualExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            ValueType valueType = lhs.GetTypePromotion(rhs);
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

    class NotEqualExpression : BinaryExpression
    {
        public NotEqualExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            ValueType valueType = lhs.GetTypePromotion(rhs);
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

    class LessExpression : BinaryExpression
    {
        public LessExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            ValueType valueType = lhs.GetTypePromotion(rhs);
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

    class LessEqualExpression : BinaryExpression
    {
        public LessEqualExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            ValueType valueType = lhs.GetTypePromotion(rhs);
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

    class GreaterExpression : BinaryExpression
    {
        public GreaterExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            ValueType valueType = lhs.GetTypePromotion(rhs);
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

    class GreaterEqualExpression : BinaryExpression
    {
        public GreaterEqualExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            ValueType valueType = lhs.GetTypePromotion(rhs);
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

    class LogicalOrExpression : BinaryExpression
    {
        public LogicalOrExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            if (lhs.ToBoolean())
            {
                return new Value(true);
            }
            Value rhs = rightExpression.Evaluate(vm, lv);
            return new Value(rhs.ToBoolean());
        }
    }

    class LogicalAndExpression : BinaryExpression
    {
        public LogicalAndExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            if (!lhs.ToBoolean())
            {
                return new Value(false);
            }
            Value rhs = rightExpression.Evaluate(vm, lv);
            return !rhs.ToBoolean() ? new Value(false) : new Value(true);
        }
    }

    class SubstituteExpression : BinaryExpression
    {
        public SubstituteExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            rhs.CopyTo(lhs);
            return new Value();
        }
    }

    class SubstituteAddExpression : BinaryExpression
    {
        public SubstituteAddExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value value;
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            ValueType valueType = lhs.GetTypePromotion(rhs);
            switch (valueType)
            {
                case ValueType.Int32:
                    value = new Value(lhs.ToInt32() + rhs.ToInt32());
                    break;
                case ValueType.Double:
                    value = new Value(lhs.ToDouble() + rhs.ToDouble());
                    break;
                case ValueType.String:
                    value = new Value(lhs.ToString() + rhs.ToString());
                    break;
                default:
                    value = new Value(lhs.ToInt32() + rhs.ToInt32());
                    break;
            }
            value.CopyTo(lhs);
            return new Value();
        }
    }

    class SubstituteSubExpression : BinaryExpression
    {
        public SubstituteSubExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value value;
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            ValueType valueType = lhs.GetTypePromotion(rhs);
            if (valueType == ValueType.Double)
            {
                value = new Value(lhs.ToDouble() - rhs.ToDouble());
            }
            else
            {
                value = new Value(lhs.ToInt32() - rhs.ToInt32());
            }
            value.CopyTo(lhs);
            return new Value();
        }

        public override bool IsOutputable
        {
            get
            {
                return false;
            }
        }
    }

    class SubstituteMulExpression : BinaryExpression
    {
        public SubstituteMulExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value value;
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            ValueType valueType = lhs.GetTypePromotion(rhs);
            if (valueType == ValueType.Double)
            {
                value = new Value(lhs.ToDouble() * rhs.ToDouble());
            }
            else
            {
                value = new Value(lhs.ToInt32() * rhs.ToInt32());
            }
            value.CopyTo(lhs);
            return new Value();
        }
    }

    class SubstituteDivExpression : BinaryExpression
    {
        public SubstituteDivExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value value;
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            ValueType valueType = lhs.GetTypePromotion(rhs);
            if (valueType == ValueType.Double)
            {
                double d_value = rhs.ToDouble();
                if (d_value != 0.0)
                {
                    value = new Value(lhs.ToDouble() / rhs.ToDouble());
                    value.CopyTo(lhs);
                }
            }
            else
            {
                int i_value = rhs.ToInt32();
                if (i_value != 0)
                {
                    value = new Value(lhs.ToInt32() / rhs.ToInt32());
                    value.CopyTo(lhs);
                }
            }
            return new Value();
        }
    }

    class ShiftLeftExpression : BinaryExpression
    {
        public ShiftLeftExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            return new Value(lhs.ToInt32() << rhs.ToInt32());
        }
    }

    class ShiftRightExpression : BinaryExpression
    {
        public ShiftRightExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            return new Value(lhs.ToInt32() >> rhs.ToInt32());
        }
    }

    class BitwiseOrExpression : BinaryExpression
    {
        public BitwiseOrExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            return new Value(lhs.ToInt32() | rhs.ToInt32());
        }
    }

    class BitwiseAndExpression : BinaryExpression
    {
        public BitwiseAndExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            return new Value(lhs.ToInt32() & rhs.ToInt32());
        }
    }

    class BitwiseXorExpression : BinaryExpression
    {
        public BitwiseXorExpression(IExpression lhs, IExpression rhs)
            : base(lhs, rhs)
        {
        }

        public override Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value lhs = leftExpression.Evaluate(vm, lv);
            Value rhs = rightExpression.Evaluate(vm, lv);
            return new Value(lhs.ToInt32() ^ rhs.ToInt32());
        }
    }

    class ValueExpression : IExpression
    {
        public ValueExpression(Value value)
        {
            this.value = value;
        }

        public Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            if (value.ValueType == ValueType.String)
            {
                string[] statements = Lexer.SplitStatement(value.ToString());
                int length = statements.Length;
                StringBuilder result = new StringBuilder();
                for (int i = 0; i < length; i++)
                {
                    if (Lexer.IsEvaluate(statements[i]))
                    {
                        IExpression expression = vm.Parser.MakeExpression(statements[i]);
                        result.Append(expression.Evaluate(vm, lv).ToString());
                    }
                    else
                    {
                        result.Append(statements[i]);
                    }
                }
                return new Value(result.ToString());
            }
            return value;
        }

        private Value value;
    }

    class EvaluateExpression : IExpression
    {
        public EvaluateExpression(string statement)
        {
            this.statements = Lexer.SplitStatement(statement);
        }

        public Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            int length = statements.Length;
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                if (Lexer.IsEvaluate(statements[i]))
                {
                    IExpression expression = vm.Parser.MakeExpression(statements[i]);
                    result.Append(expression.Evaluate(vm, lv).ToString());
                }
                else
                {
                    result.Append(statements[i]);
                }
            }
            int index = 0;
            Value value;
            string name = result.ToString();
            string[] array = Lexer.SplitArrayIndexer(name);
            string statement = array[0].Substring(1);
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

        private string[] statements;
    }

    class VariableExpression : IExpression
    {
        public VariableExpression(string statement)
        {
            this.statements = Lexer.SplitStatement(statement);
        }

        public Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            int length = statements.Length;
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                if (Lexer.IsEvaluate(statements[i]))
                {
                    IExpression expression = vm.Parser.MakeExpression(statements[i]);
                    result.Append(expression.Evaluate(vm, lv).ToString());
                }
                else
                {
                    result.Append(statements[i]);
                }
            }
            int index = 0;
            string name = result.ToString();
            string[] array = Lexer.SplitArrayIndexer(name);
            string statement = array[0].Substring(1);
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
                else
                {
                    return vm.Variables.GetVariable(statement)[index];
                }
            }
            else
            {
                if (vm.Functions.IsFunction(statement))
                {
                    return vm.Functions.ExecFunction(vm, statement);
                }
                else
                {
                    return vm.Variables.GetVariable(statement);
                }
            }
        }

        private string[] statements;
    }

    class FunctionExpression : IExpression
    {
        public FunctionExpression(string statement, IExpression[] expressions)
        {
            this.expressions = expressions;
            this.statements = Lexer.SplitStatement(statement.Substring(1));
        }

        public Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            int length = statements.Length;
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                if (Lexer.IsEvaluate(statements[i]))
                {
                    IExpression expression = vm.Parser.MakeExpression(statements[i]);
                    result.Append(expression.Evaluate(vm, lv).ToString());
                }
                else
                {
                    result.Append(statements[i]);
                }
            }
            // 関数名を確定する
            string statement = result.ToString();
            // 引数を評価する
            length = expressions.Length;
            Value[] values = new Value[length];
            for (int i = 0; i < length; i++)
            {
                values[i] = expressions[i].Evaluate(vm, lv);
            }
            if (vm.Functions.IsFunction(statement))
            {
                return vm.Functions.ExecFunction(vm, statement, values);
            }
            return new Value();
        }

        private string[] statements;
        private IExpression[] expressions;
    }

    class IfExpression : IExpression
    {
        public IfExpression(IfSubExpression[] expressions)
        {
            this.expressions = expressions;
        }

        public Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            int length = expressions.Length;
            if (length == 1 && !expressions[0].IsHaveStatements())
            {
                // 処理部省略のとき
                return new Value(expressions[0].IsExecutable(vm, lv));
            }
            for (int i = 0; i < length; i++)
            {
                if (expressions[i].IsExecutable(vm, lv))
                {
                    return expressions[i].Evaluate(vm, lv);
                }
            }
            return new Value();
        }

        private IfSubExpression[] expressions;
    }

    class IfSubExpression : IExpression
    {
        public IfSubExpression(IExpression expression,IExpression[] statements)
        {
            this.expression = expression;
            this.statements = statements;
        }

        public Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            int length = statements.Length;
            if (length == 1)
            {
                return statements[0].Evaluate(vm, lv);
            }
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                Value retval = statements[i].Evaluate(vm, lv);
                if (retval != null && retval.ValueType != ValueType.Void)
                {
                    result.Append(retval.ToString());
                }
            }
            return new Value(result.ToString());
        }

        public bool IsHaveStatements()
        {
            return statements.Length != 0;
        }

        public bool IsExecutable(MisakaVM vm, LocalVariables lv)
        {
            return expression.Evaluate(vm, lv).ToBoolean();
        }

        private IExpression expression;
        private IExpression[] statements;
    }

    class WhileExpression : IExpression
    {
        public WhileExpression(IExpression expression, IExpression[] statements)
        {
            this.expression = expression;
            this.statements = statements;
        }

        public Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            int length = statements.Length;
            StringBuilder result = new StringBuilder();
            while (expression.Evaluate(vm, lv).ToBoolean())
            {
                for (int i = 0; i < length; ++i)
                {
                    Value retval = statements[i].Evaluate(vm, lv);
                    if (retval != null && retval.ValueType != ValueType.Void)
                    {
                        result.Append(retval.ToString());
                    }
                }
            }
            return new Value(result.ToString());
        }

        private IExpression expression;
        private IExpression[] statements;
    }

    class ForExpression : IExpression
    {
        public ForExpression(IExpression[] expressions, IExpression[] statements)
        {
            this.expressions = expressions;
            this.statements = statements;
        }

        public Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            int length = statements.Length;
            StringBuilder result = new StringBuilder();
            for (expressions[0].Evaluate(vm, lv); expressions[1].Evaluate(vm, lv).ToBoolean(); expressions[2].Evaluate(vm, lv))
            {
                for (int i = 0; i < length; i++)
                {
                    Value retval = statements[i].Evaluate(vm, lv);
                    if (retval != null && retval.ValueType != ValueType.Void)
                    {
                        result.Append(retval.ToString());
                    }
                }
            }
            return new Value(result.ToString());
        }

        private IExpression[] expressions;
        private IExpression[] statements;
    }

    class ForeachExpression : IExpression
    {
        public ForeachExpression(IExpression[] expressions, IExpression[] statements)
        {
            this.expressions = expressions;
            this.statements = statements;
        }

        public Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            Value var = expressions[0].Evaluate(vm, lv);
            List<Value> Array = expressions[1].Evaluate(vm, lv).ToArray();
            int length = statements.Length;
            StringBuilder result = new StringBuilder();
            foreach (Value temp in Array)
            {
                temp.CopyTo(var);
                for (int i = 0; i < length; i++)
                {
                    Value retval = statements[i].Evaluate(vm, lv);
                    if (retval != null && retval.ValueType != ValueType.Void)
                    {
                        result.Append(retval.ToString());
                    }
                }
            }
            return new Value(result.ToString());
        }

        private IExpression[] expressions;
        private IExpression[] statements;
    }

    class TextExpression : IExpression
    {
        public TextExpression(string text)
        {
            this.text = text;
        }

        public Value Evaluate(MisakaVM vm, LocalVariables lv)
        {
            return new Value(text);
        }

        private string text;
    }
}
