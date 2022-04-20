using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ImoutoDesktop.Scripting;

internal class Variables
{
    public Encoding Encoding { get; set; } = Encoding.UTF8;

    public void Serialize(string path)
    {
        if (_variables.Count == 0)
        {
            return;
        }

        using var writer = new StreamWriter(path, false, Encoding);

        foreach (var variable in _variables.Where(variable => !variable.Value.IsEmpty))
        {
            writer.WriteLine("${0},{1}", variable.Key, variable.Value);
        }
    }

    public void Deserialize(string path)
    {
        if (!File.Exists(path))
        {
            return;
        }

        using var reader = new StreamReader(path, Encoding);
        while (reader.Peek() != -1)
        {
            var line = reader.ReadLine();
            var token = line.Split(',');
            if (token.Length < 2)
            {
                continue;
            }
            Value variable;
            var name = token[0].Substring(1);
            if (token.Length == 2)
            {
                variable = GetValue(token[1]);
            }
            else
            {
                variable = Value.Empty;
                var array = variable.ToArray();
                for (var i = 1; i < token.Length; ++i)
                {
                    array.Add(GetValue(token[i]));
                }
            }
            if (_variables.ContainsKey(name))
            {
                _variables.Remove(name);
            }
            _variables.Add(name, variable);
        }
    }

    private static Value GetValue(string value)
    {
        Value variable;
        if (int.TryParse(value, out var iValue))
        {
            variable = new Value(iValue);
        }
        else if (double.TryParse(value, out var dValue))
        {
            variable = new Value(dValue);
        }
        else if (bool.TryParse(value, out var bValue))
        {
            variable = new Value(bValue);
        }
        else
        {
            variable = new Value(value);
        }
        return variable;
    }

    public Value GetVariable(string name)
    {
        if (IsSystemVariable(name))
        {
            return GetSystemVariable(name);
        }
        if (_variables.TryGetValue(name, out var variable))
        {
            return variable;
        }
        variable = Value.Empty;
        _variables.Add(name, variable);
        return variable;
    }

    private Value GetSystemVariable(string name)
    {
        Value variable;
        switch (name)
        {
            case "age":
                variable = new Value(Age);
                break;
            case "allowoperate":
                variable = new Value(AllowRemoteOperate);
                break;
            case "connecting":
                variable = new Value(Connecting);
                break;
            case "day":
                variable = new Value(DateTime.Now.Day);
                break;
            case "dayofweek":
                switch (DateTime.Now.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        variable = new Value(0);
                        break;
                    case DayOfWeek.Monday:
                        variable = new Value(1);
                        break;
                    case DayOfWeek.Tuesday:
                        variable = new Value(2);
                        break;
                    case DayOfWeek.Wednesday:
                        variable = new Value(3);
                        break;
                    case DayOfWeek.Thursday:
                        variable = new Value(4);
                        break;
                    case DayOfWeek.Friday:
                        variable = new Value(5);
                        break;
                    case DayOfWeek.Saturday:
                        variable = new Value(6);
                        break;
                    default:
                        variable = Value.Empty;
                        break;
                }
                break;
            case "dayofyear":
                variable = new Value(DateTime.Now.DayOfYear);
                break;
            case "hour":
                variable = new Value(DateTime.Now.Hour);
                break;
            case "millisecond":
                variable = new Value(DateTime.Now.Millisecond);
                break;
            case "minute":
                variable = new Value(DateTime.Now.Minute);
                break;
            case "month":
                variable = new Value(DateTime.Now.Month);
                break;
            case "second":
                variable = new Value(DateTime.Now.Second);
                break;
            case "tsunderelevel":
                variable = new Value(TsundereLevel);
                break;
            case "year":
                variable = new Value(DateTime.Now.Year);
                break;
            case "reject":
                variable = _reject;
                break;
            default:
                variable = Value.Empty;
                break;
        }
        return variable;
    }

    public bool IsSystemVariable(string name) => Array.BinarySearch(_systemVariables, name) >= 0;

    private Value _reject = new(false);

    // システム変数
    public int Age { get; set; }

    public int TsundereLevel { get; set; }

    public bool AllowRemoteOperate { get; set; }

    public bool Connecting { get; set; }

    public bool Reject
    {
        get => _reject.ToBoolean();
        set => _reject = new Value(value);
    }

    private readonly SortedList<string, Value> _variables = new();
    private readonly string[] _systemVariables =
    {
        "age",
        "allowoperate",
        "connecting",
        "day",
        "dayofweek",
        "dayofyear",
        "hour",
        "millisecond",
        "minute",
        "month",
        "reject",
        "second",
        "tsunderelevel",
        "year"
    };
}
