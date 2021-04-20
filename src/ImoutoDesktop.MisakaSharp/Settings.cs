using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImoutoDesktop.MisakaSharp
{
    class Settings
    {
        public Settings()
        {
            encoding = Encoding.Default;
            dictionaries = new List<string>();
        }

        public void LoadSettings(string path)
        {
            using (var reader = new StreamReader(path, Encoding.Default))
            {
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
                                dictionaries.Add(line);
                            }
                        }
                    }
                    else if (token[0] == "charset")
                    {
                        encoding = Encoding.GetEncoding(token[1]);
                    }
                    else if (token[0] == "debug")
                    {
                        enableDebugLog = token[1] == "1";
                    }
                    else if (token[0] == "error")
                    {
                        enableErrorLog = token[1] == "1";
                    }
                }
            }
        }

        private Encoding encoding;

        public Encoding Encoding
        {
            get { return encoding; }
        }
        private bool enableErrorLog;

        public bool EnableErrorLog
        {
            get { return enableErrorLog; }
            set { enableErrorLog = value; }
        }
        private bool enableDebugLog;

        public bool EnableDebugLog
        {
            get { return enableDebugLog; }
            set { enableDebugLog = value; }
        }
        private List<string> dictionaries;

        public List<string> Dictionaries
        {
            get { return dictionaries; }
        }
    }
}
