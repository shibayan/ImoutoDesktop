using System;
using System.Collections.Generic;
using System.Text;

namespace ImoutoDesktop.MisakaSharp
{
    enum ValueType
    {
        Void,
        Boolean,
        Int32,
        Double,
        String,
        Array,
        Hash,
    }

    /// <summary>
    /// 値を保持するクラス
    /// </summary>
    class Value
    {
        /// <summary>
        /// 未定義型として初期化します。
        /// </summary>
        public Value()
        {
            valueType = ValueType.Void;
        }

        /// <summary>
        /// Booleanとして初期化します。
        /// </summary>
        /// <param name="value"></param>
        public Value(bool value)
        {
            b_value = value;
            valueType = ValueType.Boolean;
        }

        /// <summary>
        /// 32ビット整数型として初期化します。
        /// </summary>
        /// <param name="value"></param>
        public Value(int value)
        {
            i_value = value;
            valueType = ValueType.Int32;
        }

        /// <summary>
        /// 64ビット浮動小数型として初期化します。
        /// </summary>
        /// <param name="value"></param>
        public Value(double value)
        {
            d_value = value;
            valueType = ValueType.Double;
        }

        /// <summary>
        /// 文字列型として初期化します。
        /// </summary>
        /// <param name="value"></param>
        public Value(string value)
        {
            s_value = value;
            valueType = ValueType.String;
        }

        /// <summary>
        /// 配列型として初期化します。
        /// </summary>
        /// <param name="value"></param>
        public Value(params Value[] value)
        {
            a_value = new List<Value>();
            a_value.AddRange(value);
            valueType = ValueType.Array;
        }

        public Value(List<Value> value)
        {
            a_value = new List<Value>(value);
            valueType = ValueType.Array;
        }

        /// <summary>
        /// コピーコンストラクタ。
        /// </summary>
        /// <param name="obj"></param>
        public Value(Value obj)
        {
            obj.CopyTo(this);
        }

        /// <summary>
        /// このオブジェクトから指定されたオブジェクトへ中身をコピーします。
        /// </summary>
        /// <param name="obj">コピー先のValueオブジェクト。</param>
        public void CopyTo(Value obj)
        {
            obj.b_value = b_value;
            obj.i_value = i_value;
            obj.d_value = d_value;
            obj.s_value = s_value;
            obj.a_value = a_value;
            obj.valueType = valueType;
        }

        /// <summary>
        /// このオブジェクトのクローンを作成します。
        /// </summary>
        /// <returns>新しいオブジェクト。</returns>
        public Value Clone()
        {
            return new Value(this);
        }

        /// <summary>
        /// 配列のインデクサです。
        /// </summary>
        /// <param name="index">インデックス。</param>
        /// <returns>指定されたインデックスの値。</returns>
        public Value this[int index]
        {
            get
            {
                if (index == 0)
                {
                    switch (valueType)
                    {
                        case ValueType.Void:
                        case ValueType.Boolean:
                        case ValueType.Int32:
                        case ValueType.Double:
                            return this;
                        default:
                            break;
                    }
                }
                switch (valueType)
                {
                    case ValueType.String:
                        if (s_value.Length > index)
                        {
                            return new Value((int)s_value[index]);
                        }
                        break;
                    case ValueType.Array:
                        if (a_value.Count > index)
                        {
                            return a_value[index];
                        }
                        break;
                }
                return new Value();
            }
            set
            {
                a_value[index] = value;
            }
        }

        private ValueType valueType;

        /// <summary>
        /// 現在保持している値の型を取得します。
        /// </summary>
        internal ValueType ValueType
        {
            get { return valueType; }
        }

        /// <summary>
        /// 型の昇格を行います。
        /// </summary>
        /// <param name="value">調べるValueオブジェクト。</param>
        /// <returns></returns>
        public ValueType GetTypePromotion(Value value)
        {
            return valueType > value.valueType ? valueType : value.valueType;
        }

        /// <summary>
        /// このValueオブジェクトが空かどうか判別する値を取得します。
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                switch (valueType)
                {
                    case ValueType.Boolean:
                    case ValueType.Int32:
                    case ValueType.Double:
                        return false;
                    case ValueType.String:
                        return string.IsNullOrEmpty(s_value);
                    case ValueType.Array:
                        return a_value == null || a_value.Count == 0;
                    case ValueType.Void:
                    default:
                        return true;
                }
            }
        }

        /// <summary>
        /// このValueオブジェクトに含まれている要素の数を取得します。
        /// </summary>
        public int Count
        {
            get
            {
                switch (valueType)
                {
                    case ValueType.Boolean:
                    case ValueType.Int32:
                    case ValueType.Double:
                    case ValueType.String:
                        return 1;
                    case ValueType.Array:
                        return a_value.Count;
                    case ValueType.Void:
                    default:
                        return 0;
                }
            }
        }

        /// <summary>
        /// 保持している値をSystem.Int32に変換して出力します。
        /// </summary>
        /// <returns>変換した値。</returns>
        public int ToInt32()
        {
            switch (valueType)
            {
                case ValueType.Boolean:
                    return b_value == true ? 1 : 0;
                case ValueType.Int32:
                    return i_value;
                case ValueType.Double:
                    return (int)d_value;
                case ValueType.String:
                    int retval = 0;
                    int.TryParse(s_value, out retval);
                    return retval;
                case ValueType.Array:
                case ValueType.Void:
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 保持している値をSystem.Doubleに変換して出力します。
        /// </summary>
        /// <returns>変換した値。</returns>
        public double ToDouble()
        {
            switch (valueType)
            {
                case ValueType.Boolean:
                    return b_value ? 1.0 : 0.0;
                case ValueType.Int32:
                    return i_value;
                case ValueType.Double:
                    return d_value;
                case ValueType.String:
                    double retval = 0.0;
                    double.TryParse(s_value, out retval);
                    return retval;
                case ValueType.Array:
                case ValueType.Void:
                default:
                    return 0.0;
            }
        }

        /// <summary>
        /// 保持している値をSystem.Booleanに変換して出力します。
        /// </summary>
        /// <returns>変換した値。</returns>
        public bool ToBoolean()
        {
            switch (valueType)
            {
                case ValueType.Boolean:
                    return b_value;
                case ValueType.Int32:
                    return i_value != 0;
                case ValueType.Double:
                    return d_value != 0.0;
                case ValueType.String:
                    return string.IsNullOrEmpty(s_value) == false && s_value != "false";
                case ValueType.Array:
                case ValueType.Void:
                default:
                    return false;
            }
        }

        /// <summary>
        /// 保持している値をSystem.Stringに変換して出力します。
        /// </summary>
        /// <returns>変換した値。</returns>
        public override string ToString()
        {
            switch (valueType)
            {
                case ValueType.Boolean:
                    return b_value ? "true" : "false";
                case ValueType.Int32:
                    return i_value.ToString();
                case ValueType.Double:
                    return d_value.ToString();
                case ValueType.String:
                    return s_value;
                case ValueType.Array:
                    if (a_value != null)
                    {
                        int length = a_value.Count;
                        StringBuilder retstr = new StringBuilder();
                        for (int i = 0; i < length; ++i)
                        {
                            retstr.Append(a_value[i].ToString());
                            retstr.Append(',');
                        }
                        retstr.Remove(retstr.Length - 1, 1);
                        return retstr.ToString();
                    }
                    return string.Empty;
                case ValueType.Void:
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 保持している値を配列に変換して出力します。
        /// </summary>
        /// <returns>変換した値。</returns>
        public List<Value> ToArray()
        {
            if (a_value == null)
            {
                valueType = ValueType.Array;
                a_value = new List<Value>();
                switch (valueType)
                {
                    case ValueType.Boolean:
                        a_value.Add(new Value(b_value));
                        break;
                    case ValueType.Int32:
                        a_value.Add(new Value(i_value));
                        break;
                    case ValueType.Double:
                        a_value.Add(new Value(d_value));
                        break;
                    case ValueType.String:
                        a_value.Add(new Value(s_value));
                        break;
                }
            }
            return a_value;
        }

        private int i_value;
        private bool b_value;
        private string s_value;
        private double d_value;
        private List<Value> a_value;
    }
}
