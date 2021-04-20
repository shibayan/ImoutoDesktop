using System.Linq;

using ImoutoDesktop.IO;
using ImoutoDesktop.Remoting;

namespace ImoutoDesktop.Commands
{
    public class CommandManager
    {
        public CommandManager(Character character, RemoteConnectionManager remoteConnectionManager)
        {
            _commands = new CommandBase[]
            {
                new CallName(character.Name, remoteConnectionManager),
                new Connect(remoteConnectionManager),
                new Disconnect(remoteConnectionManager),
                new ChangeDirectory(remoteConnectionManager),
                new DosCommand(remoteConnectionManager),
                new ExecuteFile(remoteConnectionManager),
                new OpenFile(remoteConnectionManager),
                new DeleteFile(remoteConnectionManager),
                new ScreenShot(remoteConnectionManager),
                new Close(remoteConnectionManager)
            };
        }

        private readonly CommandBase[] _commands;

        public CommandBase Get(string input)
        {
            return _commands.OrderByDescending(p => p.Priority).FirstOrDefault(p => p.IsExecute(input));
        }
    }
}
