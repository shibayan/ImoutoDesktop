using System;
using System.Collections.Generic;

using ImoutoDesktop.Remoting;

namespace ImoutoDesktop.Commands
{
    public class DosCommand : CommandBase
    {
        public DosCommand()
            : base(@".+")
        {
        }

        private string _directory;

        private static readonly string[] _allow = new string[]
        {
            "dir", "ver", "xcopy", "mkdir", "rmdir", "copy", "del", "move", "ren", "cd", "type", "ls", "chdir", "rm", "cp", "cls", "start"
        };

        private static readonly Dictionary<string, string> _replace = new()
        {
            { "chdir", "cd" }, { "ls", "dir" }, { "rm", "del" }, { "cp", "copy" }
        };

        public override Priority Priority
        {
            get { return Priority.BelowNormal; }
        }

        public override bool IsExecute(string input)
        {
            return Array.Exists(_allow, p => input == p || input.StartsWith(p + " ")) && ConnectionPool.IsConnected;
        }

        public override bool PreExecute(string input)
        {
            EventID = null;

            if (input.StartsWith("cd"))
            {
                EventID = "ChangeDirectory";
                input = input.Trim();

                if (input.Length < 3)
                {
                    Parameters = new[] { Escape(input), "unknown" };
                    return false;
                }

                _directory = AbsolutePath(ConnectionPool.Connection.CurrentDirectory, input.Substring(2).Trim());

                if (!_directory.EndsWith(@"\"))
                {
                    _directory += @"\";
                }

                var type = DirectoryType.None;

                if (Settings.Default.AutoDetectDirectoryType)
                {
                    type = ConnectionPool.Connection.GetDirectoryType(_directory);
                }

                Parameters = new[] { Escape(_directory), Enum.GetName(typeof(DirectoryType), type) };

                return true;
            }

            Parameters = new[] { Escape(input) };

            return true;
        }

        public override bool Execute(string input, out string result)
        {
            EventID = null;

            foreach (var item in _replace)
            {
                if (input.StartsWith(item.Key + " "))
                {
                    input = item.Key + input.Substring(item.Key.Length);
                    break;
                }
            }

            if (input.StartsWith("cd"))
            {
                result = null;
                EventID = "ChangeDirectory";

                if (ConnectionPool.Connection.Exists(_directory) != Exists.Directory)
                {
                    Parameters = new[] { Escape(_directory), "not exist" };
                    return false;
                }

                try
                {
                    ConnectionPool.Connection.CurrentDirectory = _directory;
                }
                catch
                {
                    Parameters = new[] { Escape(_directory), "unknown" };
                    return false;
                }

                return true;
            }

            if (input.StartsWith("cls"))
            {
                result = null;
                return true;
            }

            result = ConnectionPool.Connection.ExecuteCommand(input);

            return true;
        }
    }
}
