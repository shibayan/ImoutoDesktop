using System.Collections.Generic;

namespace ImoutoDesktop.MisakaSharp
{
    class LocalVariables
    {
        public LocalVariables()
        {
            variables = new SortedList<string, Value>();
        }

        public void AddVariable(string name, Value value)
        {
            variables.Add(name, value);
        }

        public Value GetVariable(string name)
        {
            Value variable;
            if (variables.TryGetValue(name, out variable))
            {
                return variable;
            }
            variable = new Value();
            variables.Add(name, variable);
            return variable;
        }

        public static bool IsLocalVariable(string name)
        {
            return name.StartsWith("@");
        }

        private SortedList<string, Value> variables;
    }
}
