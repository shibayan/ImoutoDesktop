using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using ImoutoDesktop.Remoting;

namespace ImoutoDesktop.Commands
{
    public static class CommandManager
    {
        private static readonly List<ICommand> _registedCommands = new List<ICommand>();
        private static readonly List<ITranslate> _registedTranslators = new List<ITranslate>();

        public static void Rebuild(string directory)
        {
            _registedCommands.Clear();
            _registedTranslators.Clear();

            // システムコマンドを登録
            _registedCommands.AddRange(new ICommand[]
            {
                new Connect(),
                new ChangeDirectory(),
                new DosCommand(),
                new ExecuteFile(),
                new OpenFile(),
                new DeleteFile(),
                new ScreenShot(),
                new ExitCommand()
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

                    command.Initialize(directory);

                    _registedCommands.Add(command);
                }

                foreach (var type in types.Where(p => p.GetInterface(typeof(ITranslate).FullName) != null))
                {
                    var translate = (ITranslate)Activator.CreateInstance(type);

                    if (translate == null)
                    {
                        continue;
                    }

                    translate.Initialize(directory);

                    _registedTranslators.Add(translate);
                }
            }
        }

        public static void Shutdown()
        {
            foreach (var command in _registedCommands)
            {
                command.Uninitialize();
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

        public static void ExecuteTranslate(ref string text)
        {
            foreach (var translator in _registedTranslators.OrderByDescending(p => p.Priority))
            {
                if (translator.Execute(text, out var result))
                {
                    text = result;
                }
            }
        }
    }
}
