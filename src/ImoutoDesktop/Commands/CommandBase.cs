using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ImoutoDesktop.Services;

namespace ImoutoDesktop.Commands
{
    public abstract class CommandBase
    {
        protected CommandBase(string pattern, RemoteConnectionManager remoteConnectionManager)
        {
            Pattern = new Regex(pattern);
            RemoteConnectionManager = remoteConnectionManager;
        }

        public virtual Priority Priority => Priority.Normal;

        public virtual bool IsExecute(string input)
        {
            return Pattern.IsMatch(input);
        }

        public virtual Task<CommandResult> PreExecute(string input)
        {
            return Task.FromResult(Succeeded());
        }

        public virtual Task<CommandResult> Execute(string input)
        {
            return Task.FromResult(Succeeded());
        }

        protected Regex Pattern { get; }

        protected RemoteConnectionManager RemoteConnectionManager { get; }

        protected CommandResult Failed()
        {
            return Failed(null, null);
        }

        protected CommandResult Failed(string message)
        {
            return Failed(message, null);
        }

        protected CommandResult Failed(string[] arguments)
        {
            return Failed(null, arguments);
        }

        protected CommandResult Failed(string message, string[] arguments)
        {
            return new()
            {
                EventId = GetType().Name + "Failure",
                Arguments = arguments,
                Message = message
            };
        }

        protected CommandResult Succeeded()
        {
            return Succeeded(null, null);
        }

        protected CommandResult Succeeded(string message)
        {
            return Succeeded(message, null);
        }

        protected CommandResult Succeeded(string[] arguments)
        {
            return Succeeded(null, arguments);
        }

        protected CommandResult Succeeded(string message, string[] arguments)
        {
            return new()
            {
                EventId = GetType().Name,
                Arguments = arguments,
                Message = message
            };
        }

        protected string Escape(string str)
        {
            return str.Replace(@"\", @"\\");
        }

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
