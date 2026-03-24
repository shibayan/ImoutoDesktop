using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
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

        foreach (var (name, value) in _variables)
        {
            if (value.IsEmpty)
            {
                continue;
            }

            writer.WriteLine("${0},{1}", name, value);
        }
    }

    public void Deserialize(string path)
    {
        if (!File.Exists(path))
        {
            return;
        }

        using var reader = new StreamReader(path, Encoding);
        while (reader.ReadLine() is { } line)
        {
            var token = line.Split(',');
            if (token.Length < 2)
            {
                continue;
            }
            Value variable;
            var name = token[0][1..];
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
            _variables[name] = variable;
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

        return _variables[name] = Value.Empty;
    }

    private Value GetSystemVariable(string name)
    {
        var now = DateTime.Now;

        return name switch
        {
            "age" => new Value(Age),
            "allowoperate" => new Value(AllowRemoteOperate),
            "connecting" => new Value(Connecting),
            "day" => new Value(now.Day),
            "dayofweek" => new Value((int)now.DayOfWeek),
            "dayofyear" => new Value(now.DayOfYear),
            "hour" => new Value(now.Hour),
            "millisecond" => new Value(now.Millisecond),
            "minute" => new Value(now.Minute),
            "month" => new Value(now.Month),
            "reject" => _reject,
            "second" => new Value(now.Second),
            "tsunderelevel" => new Value(TsundereLevel),
            "year" => new Value(now.Year),
            _ => Value.Empty
        };
    }

    public bool IsSystemVariable(string name) => s_systemVariables.Contains(name);

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
    private static readonly FrozenSet<string> s_systemVariables = new[]
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
    }.ToFrozenSet(StringComparer.Ordinal);
}
