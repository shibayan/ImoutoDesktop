using System.Collections.Generic;

namespace ImoutoDesktop.Scripting;

internal enum ValueType
{
    Void,
    Boolean,
    Int32,
    Double,
    String,
    Array
}

/// <summary>
/// 値を保持するクラス
/// </summary>
internal class Value
{
    /// <summary>
    /// 未定義型として初期化します。
    /// </summary>
    private Value()
    {
        ValueType = ValueType.Void;
    }

    /// <summary>
    /// Booleanとして初期化します。
    /// </summary>
    /// <param name="value"></param>
    public Value(bool value)
    {
        _bValue = value;
        ValueType = ValueType.Boolean;
    }

    /// <summary>
    /// 32ビット整数型として初期化します。
    /// </summary>
    /// <param name="value"></param>
    public Value(int value)
    {
        _iValue = value;
        ValueType = ValueType.Int32;
    }

    /// <summary>
    /// 64ビット浮動小数型として初期化します。
    /// </summary>
    /// <param name="value"></param>
    public Value(double value)
    {
        _dValue = value;
        ValueType = ValueType.Double;
    }

    /// <summary>
    /// 文字列型として初期化します。
    /// </summary>
    /// <param name="value"></param>
    public Value(string value)
    {
        _value = value;
        ValueType = ValueType.String;
    }

    /// <summary>
    /// 配列型として初期化します。
    /// </summary>
    /// <param name="value"></param>
    public Value(IEnumerable<Value> value)
    {
        _aValue = new List<Value>(value);
        ValueType = ValueType.Array;
    }

    /// <summary>
    /// コピーコンストラクタ。
    /// </summary>
    /// <param name="obj"></param>
    public Value(Value obj)
    {
        obj.CopyTo(this);
    }

    public static Value Empty { get; } = new();

    /// <summary>
    /// このオブジェクトから指定されたオブジェクトへ中身をコピーします。
    /// </summary>
    /// <param name="obj">コピー先のValueオブジェクト。</param>
    public void CopyTo(Value obj)
    {
        obj._bValue = _bValue;
        obj._iValue = _iValue;
        obj._dValue = _dValue;
        obj._value = _value;
        obj._aValue = _aValue;
        obj.ValueType = ValueType;
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
                switch (ValueType)
                {
                    case ValueType.Void:
                    case ValueType.Boolean:
                    case ValueType.Int32:
                    case ValueType.Double:
                        return this;
                }
            }
            switch (ValueType)
            {
                case ValueType.String:
                    if (_value.Length > index)
                    {
                        return new Value((int)_value[index]);
                    }
                    break;
                case ValueType.Array:
                    if (_aValue.Count > index)
                    {
                        return _aValue[index];
                    }
                    break;
            }
            return new Value();
        }
        set
        {
            _aValue[index] = value;
        }
    }

    /// <summary>
    /// 現在保持している値の型を取得します。
    /// </summary>
    internal ValueType ValueType { get; private set; }

    /// <summary>
    /// 型の昇格を行います。
    /// </summary>
    /// <param name="value">調べるValueオブジェクト。</param>
    /// <returns></returns>
    public ValueType GetTypePromotion(Value value) => ValueType > value.ValueType ? ValueType : value.ValueType;

    /// <summary>
    /// このValueオブジェクトが空かどうか判別する値を取得します。
    /// </summary>
    public bool IsEmpty
    {
        get
        {
            return ValueType switch
            {
                ValueType.Boolean => false,
                ValueType.Int32 => false,
                ValueType.Double => false,
                ValueType.String => string.IsNullOrEmpty(_value),
                ValueType.Array => _aValue == null || _aValue.Count == 0,
                ValueType.Void => true,
                _ => true
            };
        }
    }

    /// <summary>
    /// このValueオブジェクトに含まれている要素の数を取得します。
    /// </summary>
    public int Count
    {
        get
        {
            return ValueType switch
            {
                ValueType.Boolean => 1,
                ValueType.Int32 => 1,
                ValueType.Double => 1,
                ValueType.String => 1,
                ValueType.Array => _aValue.Count,
                ValueType.Void => 0,
                _ => 0
            };
        }
    }

    /// <summary>
    /// 保持している値をSystem.Int32に変換して出力します。
    /// </summary>
    /// <returns>変換した値。</returns>
    public int ToInt32()
    {
        return ValueType switch
        {
            ValueType.Boolean => _bValue ? 1 : 0,
            ValueType.Int32 => _iValue,
            ValueType.Double => (int)_dValue,
            ValueType.String => int.TryParse(_value, out var retval) ? retval : 0,
            ValueType.Array => 0,
            ValueType.Void => 0,
            _ => 0
        };
    }

    /// <summary>
    /// 保持している値をSystem.Doubleに変換して出力します。
    /// </summary>
    /// <returns>変換した値。</returns>
    public double ToDouble()
    {
        return ValueType switch
        {
            ValueType.Boolean => _bValue ? 1.0 : 0.0,
            ValueType.Int32 => _iValue,
            ValueType.Double => _dValue,
            ValueType.String => double.TryParse(_value, out var retval) ? retval : 0.0,
            ValueType.Array => 0.0,
            ValueType.Void => 0.0,
            _ => 0.0
        };
    }

    /// <summary>
    /// 保持している値をSystem.Booleanに変換して出力します。
    /// </summary>
    /// <returns>変換した値。</returns>
    public bool ToBoolean()
    {
        return ValueType switch
        {
            ValueType.Boolean => _bValue,
            ValueType.Int32 => _iValue != 0,
            ValueType.Double => _dValue != 0.0,
            ValueType.String => string.IsNullOrEmpty(_value) == false && _value != "false",
            ValueType.Array => false,
            ValueType.Void => false,
            _ => false
        };
    }

    /// <summary>
    /// 保持している値をSystem.Stringに変換して出力します。
    /// </summary>
    /// <returns>変換した値。</returns>
    public override string ToString()
    {
        return ValueType switch
        {
            ValueType.Boolean => _bValue ? "true" : "false",
            ValueType.Int32 => _iValue.ToString(),
            ValueType.Double => _dValue.ToString(),
            ValueType.String => _value,
            ValueType.Array => _aValue is not null ? string.Join(",", _aValue) : string.Empty,
            ValueType.Void => string.Empty,
            _ => string.Empty
        };
    }

    /// <summary>
    /// 保持している値を配列に変換して出力します。
    /// </summary>
    /// <returns>変換した値。</returns>
    public List<Value> ToArray()
    {
        if (_aValue != null)
        {
            return _aValue;
        }

        ValueType = ValueType.Array;
        _aValue = new List<Value>();

        switch (ValueType)
        {
            case ValueType.Boolean:
                _aValue.Add(new Value(_bValue));
                break;
            case ValueType.Int32:
                _aValue.Add(new Value(_iValue));
                break;
            case ValueType.Double:
                _aValue.Add(new Value(_dValue));
                break;
            case ValueType.String:
                _aValue.Add(new Value(_value));
                break;
        }

        return _aValue;
    }

    private int _iValue;
    private bool _bValue;
    private string _value;
    private double _dValue;
    private List<Value> _aValue;
}
