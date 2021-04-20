using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ImoutoDesktop.Commands
{
    public static class CommandManager
    {
        private static readonly List<ICommand> _registedCommands = new();
        private static readonly List<ITranslator> _registedTranslators = new();

        public static void Rebuild(string directory)
        {
            _registedCommands.Clear();
            _registedTranslators.Clear();

            // システムコマンドを登録
            _registedCommands.AddRange(new ICommand[]
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

            // 外部コマンドを登録
            foreach (var file in Directory.GetFiles(directory, "*.dll"))
            {
                var assembly = Assembly.LoadFile(file);
                var types = assembly.GetExportedTypes();

                foreach (var type in types.Where(p => p.GetInterface(typeof(ICommand).FullName) != null))
                {
                    var command = (ICommand)Activator.CreateInstance(type);

                    if (command == null)
                    {
                        continue;
                    }

                    _registedCommands.Add(command);
                }

                foreach (var type in types.Where(p => p.GetInterface(typeof(ITranslator).FullName) != null))
                {
                    var translate = (ITranslator)Activator.CreateInstance(type);

                    if (translate == null)
                    {
                        continue;
                    }

                    _registedTranslators.Add(translate);
                }
            }
        }

        public static ICommand Get(string input)
        {
            return _registedCommands.OrderByDescending(p => p.Priority).FirstOrDefault(p => p.IsExecute(input));
        }

        public static void Add(ICommand command)
        {
            _registedCommands.Add(command);
        }

        public static void Remove(ICommand command)
        {
            _registedCommands.Remove(command);
        }

        public static string ExecuteTranslator(string text)
        {
            return _registedTranslators.OrderByDescending(p => p.Priority).Aggregate(text, (current, translator) => translator.Translate(current));
        }
    }
}
