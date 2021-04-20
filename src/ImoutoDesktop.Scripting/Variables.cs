using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImoutoDesktop.Scripting
{
    class Variables
    {
        public Variables()
        {
            encoding = Encoding.UTF8;
            variables = new SortedList<string, Value>();
        }

        private Encoding encoding;

        public Encoding Encoding
        {
            set { encoding = value; }
        }

        public void Serialize(string path)
        {
            if (variables.Count == 0)
            {
                return;
            }
            using (var writer = new StreamWriter(path, false, encoding))
            {
                foreach (var variable in variables)
                {
                    if (!variable.Value.IsEmpty)
                    {
                        writer.WriteLine("${0},{1}", variable.Key, variable.Value.ToString());
                    }
                }
            }
        }

        public void Deserialize(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }
            using (var reader = new StreamReader(path, encoding))
            {
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
                        variable = new Value();
                        var array = variable.ToArray();
                        for (var i = 1; i < token.Length; ++i)
                        {
                            array.Add(GetValue(token[i]));
                        }
                    }
                    if (variables.ContainsKey(name))
                    {
                        variables.Remove(name);
                    }
                    variables.Add(name, variable);
                }
            }
        }

        private static Value GetValue(string s_value)
        {
            int i_value;
            bool b_value;
            double d_value;
            Value variable;
            if (int.TryParse(s_value, out i_value))
            {
                variable = new Value(i_value);
            }
            else if (double.TryParse(s_value, out d_value))
            {
                variable = new Value(d_value);
            }
            else if (bool.TryParse(s_value, out b_value))
            {
                variable = new Value(b_value);
            }
            else
            {
                variable = new Value(s_value);
            }
            return variable;
        }

        public Value GetVariable(string name)
        {
            Value variable;
            if (IsSystemVariable(name))
            {
                return GetSystemVariable(name);
            }
            if (variables.TryGetValue(name, out variable))
            {
                return variable;
            }
            variable = new Value();
            variables.Add(name, variable);
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
                            variable = new Value();
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
                    variable = reject;
                    break;
                default:
                    variable = new Value();
                    break;
            }
            return variable;
        }

        public bool IsSystemVariable(string name)
        {
            return Array.BinarySearch<string>(systemVariables, name) >= 0;
        }

        private Value reject = new Value(false);

        // システム変数
        public int Age { get; set; }

        public int TsundereLevel { get; set; }

        public bool AllowRemoteOperate { get; set; }

        public bool Connecting { get; set; }

        public bool Reject
        {
            get { return reject.ToBoolean(); }
            set { reject = new Value(value); }
        }

        private SortedList<string, Value> variables;
        private string[] systemVariables = new string[]
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
}
