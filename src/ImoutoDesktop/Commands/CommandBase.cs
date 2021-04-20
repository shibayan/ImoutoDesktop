using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ImoutoDesktop.Commands
{
    public abstract class CommandBase : ICommand
    {
        protected CommandBase(string pattern)
        {
            _pattern = new Regex(pattern);
        }

        protected readonly Regex _pattern;

        #region ICommand メンバ

        public virtual Priority Priority
        {
            get { return Priority.Normal; }
        }

        public string EventID { get; set; }

        public string[] Parameters { get; set; }

        public virtual void Initialize(string path)
        {
        }

        public virtual void Uninitialize()
        {
        }

        public virtual bool IsExecute(string input)
        {
            return _pattern.IsMatch(input);
        }

        public abstract bool PreExecute(string input);

        public abstract bool Execute(string input, out string result);

        #endregion

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
