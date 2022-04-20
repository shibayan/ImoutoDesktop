using System.Collections.Generic;

namespace ImoutoDesktop.Scripting;

internal class LocalVariables
{
    public void AddVariable(string name, Value value)
    {
        _variables.Add(name, value);
    }

    public Value GetVariable(string name)
    {
        if (_variables.TryGetValue(name, out var variable))
        {
            return variable;
        }
        variable = Value.Empty;
        _variables.Add(name, variable);
        return variable;
    }

    public static bool IsLocalVariable(string name)
    {
        return name.StartsWith("@");
    }

    private readonly SortedList<string, Value> _variables = new();
}
