using System.Collections.Generic;
using System.Linq;

namespace ImoutoDesktop.Commands
{
    public static class CommandManager
    {
        private static readonly List<CommandBase> _registedCommands = new();

        public static void Rebuild(string directory)
        {
            _registedCommands.Clear();

            // システムコマンドを登録
            _registedCommands.AddRange(new CommandBase[]
            {
                new Connect(),
                new Disconnect(),
                new ChangeDirectory(),
                new DosCommand(),
                new ExecuteFile(),
                new OpenFile(),
                new DeleteFile(),
                new ScreenShot(),
                new Close()
            });
        }

        public static CommandBase Get(string input)
        {
            return _registedCommands.OrderByDescending(p => p.Priority).FirstOrDefault(p => p.IsExecute(input));
        }

        public static void Add(CommandBase command)
        {
            _registedCommands.Add(command);
        }

        public static void Remove(CommandBase command)
        {
            _registedCommands.Remove(command);
        }
    }
}
