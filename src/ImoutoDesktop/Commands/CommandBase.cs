using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ImoutoDesktop.Commands
{
    public abstract class CommandBase : ICommand
    {
        protected CommandBase(string pattern)
        {
            Pattern = new Regex(pattern);
        }

        protected Regex Pattern { get; }

        #region ICommand メンバ

        public virtual Priority Priority
        {
            get { return Priority.Normal; }
        }

        public string EventID { get; set; }

        public string[] Parameters { get; set; }

        public virtual bool IsExecute(string input)
        {
            return Pattern.IsMatch(input);
        }

        public virtual bool PreExecute(string input)
        {
            return true;
        }

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
