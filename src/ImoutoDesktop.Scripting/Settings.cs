using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImoutoDesktop.Scripting;

internal class Settings
{
    public void LoadSettings(string path)
    {
        using var reader = new StreamReader(path, Encoding.Default);
        while (reader.Peek() != -1)
        {
            var line = reader.ReadLine().Trim();
            if (line.Length == 0)
            {
                continue;
            }
            var token = line.Split(',');
            if (token[0] == "dictionary")
            {
                while (reader.Peek() != -1)
                {
                    line = reader.ReadLine().Trim();
                    if (line == "{")
                    {
                    }
                    else if (line == "}")
                    {
                        break;
                    }
                    else
                    {
                        Dictionaries.Add(line);
                    }
                }
            }
            else if (token[0] == "charset")
            {
                Encoding = Encoding.GetEncoding(token[1]);
            }
            else if (token[0] == "debug")
            {
                EnableDebugLog = token[1] == "1";
            }
            else if (token[0] == "error")
            {
                EnableErrorLog = token[1] == "1";
            }
        }
    }

    public Encoding Encoding { get; private set; } = Encoding.UTF8;

    public bool EnableErrorLog { get; set; }

    public bool EnableDebugLog { get; set; }

    public List<string> Dictionaries { get; } = new();
}
