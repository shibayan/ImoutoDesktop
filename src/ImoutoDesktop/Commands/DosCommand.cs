using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ImoutoDesktop.Remoting;
using ImoutoDesktop.Services;

namespace ImoutoDesktop.Commands
{
    public class DosCommand : CommandBase
    {
        public DosCommand(RemoteConnectionManager remoteConnectionManager)
            : base(@".+", remoteConnectionManager)
        {
        }

        private static readonly string[] _allow =
        {
            "dir", "ver", "xcopy", "mkdir", "rmdir", "copy", "del", "move", "ren", "cd", "type", "ls", "chdir", "rm", "cp", "cls", "start"
        };

        private static readonly Dictionary<string, string> _replace = new()
        {
            { "chdir", "cd" },
            { "ls", "dir" },
            { "rm", "del" },
            { "cp", "copy" }
        };

        public override Priority Priority => Priority.BelowNormal;

        public override bool IsExecute(string input)
        {
            return Array.Exists(_allow, p => input == p || input.StartsWith(p + " "));
        }

        public override Task<CommandResult> PreExecute(string input)
        {
            return Task.FromResult(Succeeded(new[] { Escape(input) }));
        }

        public override async Task<CommandResult> Execute(string input)
        {
            var serviceClient = RemoteConnectionManager.GetServiceClient();

            foreach (var item in _replace)
            {
                if (input.StartsWith(item.Key + " "))
                {
                    input = item.Key + input.Substring(item.Key.Length);
                    break;
                }
            }

            var response = await serviceClient.RunShellAsync(new RunShellRequest { Command = input });

            return Succeeded(response.Result);
        }
    }
}
