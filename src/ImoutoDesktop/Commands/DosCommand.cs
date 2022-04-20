using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ImoutoDesktop.Remoting;
using ImoutoDesktop.Services;

namespace ImoutoDesktop.Commands;

public class DosCommand : RemoteCommandBase
{
    public DosCommand(RemoteConnectionManager remoteConnectionManager)
        : base(@".+", remoteConnectionManager)
    {
    }

    private static readonly string[] s_allow =
    {
        "dir", "ver", "xcopy", "mkdir", "rmdir", "copy", "del", "move", "ren", "cd", "type", "ls", "chdir", "rm", "cp", "cls", "start"
    };

    private static readonly Dictionary<string, string> s_replace = new()
    {
        { "chdir", "cd" },
        { "ls", "dir" },
        { "rm", "del" },
        { "cp", "copy" }
    };

    public override Priority Priority => Priority.BelowNormal;

    public override bool CanExecute(string input)
    {
        return Array.Exists(s_allow, p => input == p || input.StartsWith(p + " "));
    }

    protected override Task<CommandResult> PreExecuteCore(string input)
    {
        return Task.FromResult(Succeeded(new[] { Escape(input) }));
    }

    protected override async Task<CommandResult> ExecuteCore(string input)
    {
        var serviceClient = RemoteConnectionManager.GetServiceClient();

        foreach (var item in s_replace)
        {
            if (input.StartsWith(item.Key + " "))
            {
                input = item.Key + input.Substring(item.Key.Length);
                break;
            }
        }

        var response = await serviceClient.RunShellAsync(new RunShellRequest { Command = input });

        return Succeeded(response.Result, new[] { Escape(input) });
    }
}
