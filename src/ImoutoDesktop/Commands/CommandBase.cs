using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImoutoDesktop.Commands
{
    public abstract class CommandBase
    {
        protected CommandBase(string pattern)
        {
            Pattern = new Regex(pattern);
        }

        public virtual Priority Priority => Priority.Normal;

        public virtual bool CanExecute(string input) => Pattern.IsMatch(input);

        public virtual Task<CommandResult> PreExecute(string input) => Task.FromResult(Succeeded());

        public virtual Task<CommandResult> Execute(string input) => Task.FromResult(Succeeded());

        protected Regex Pattern { get; }

        protected CommandResult Failed() => Failed(null, null);

        protected CommandResult Failed(string message) => Failed(message, null);

        protected CommandResult Failed(string[] arguments) => Failed(null, arguments);

        protected CommandResult Failed(string message, string[] arguments)
        {
            return new()
            {
                EventId = GetType().Name + "Failure",
                Arguments = arguments,
                Message = message
            };
        }

        protected CommandResult Succeeded() => Succeeded(null, null);

        protected CommandResult Succeeded(string message) => Succeeded(message, null);

        protected CommandResult Succeeded(string[] arguments) => Succeeded(null, arguments);

        protected CommandResult Succeeded(string message, string[] arguments)
        {
            return new()
            {
                EventId = GetType().Name,
                Arguments = arguments,
                Message = message
            };
        }

        protected string Escape(string str) => str.Replace(@"\", @"\\");

        protected string AbsolutePath(string directory, string path)
        {
            if (Path.IsPathRooted(path))
            {
                directory = path;
            }
            else
            {
                var u1 = new Uri(directory.EndsWith(@"\") ? directory : directory + @"\");
                var u2 = new Uri(u1, path);

                directory = u2.LocalPath;
            }

            return directory;
        }
    }
}
