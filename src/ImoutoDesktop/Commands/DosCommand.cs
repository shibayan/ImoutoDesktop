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

        private static readonly string[] _allow = new string[]
        {
            "dir", "ver", "xcopy", "mkdir", "rmdir", "copy", "del", "move", "ren", "cd", "type", "ls", "chdir", "rm", "cp", "cls", "start"
        };

        private static readonly Dictionary<string, string> _replace = new()
        {
            { "chdir", "cd" }, { "ls", "dir" }, { "rm", "del" }, { "cp", "copy" }
        };

        public override Priority Priority => Priority.BelowNormal;

        public override bool IsExecute(string input)
        {
            return Array.Exists(_allow, p => input == p || input.StartsWith(p + " ")) && ConnectionPool.IsConnected;
        }

        public override CommandResult PreExecute(string input)
        {
            return Succeeded(new[] { Escape(input) });
        }

        public override CommandResult Execute(string input)
        {
            foreach (var item in _replace)
            {
                if (input.StartsWith(item.Key + " "))
                {
                    input = item.Key + input.Substring(item.Key.Length);
                    break;
                }
            }

            var result = ConnectionPool.Connection.ExecuteCommand(input);

            return Succeeded(result);
        }
    }
}
