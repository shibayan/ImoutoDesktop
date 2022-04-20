using System.Linq;

using ImoutoDesktop.Commands;
using ImoutoDesktop.Models;

namespace ImoutoDesktop.Services;

public class RemoteCommandManager
{
    public RemoteCommandManager(Character character, RemoteConnectionManager remoteConnectionManager)
    {
        _commands = new CommandBase[]
        {
            new CallName(character.Name),
            new Connect(remoteConnectionManager),
            new Disconnect(remoteConnectionManager),
            new ChangeDirectory(remoteConnectionManager),
            new DosCommand(remoteConnectionManager),
            new ExecuteFile(remoteConnectionManager),
            new OpenFile(remoteConnectionManager),
            new DeleteFile(remoteConnectionManager),
            new ScreenShot(remoteConnectionManager),
            new Close()
        };
    }

    private readonly CommandBase[] _commands;

    public CommandBase Get(string input) => _commands.OrderByDescending(p => p.Priority).FirstOrDefault(p => p.CanExecute(input));
}
