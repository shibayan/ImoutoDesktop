using ImoutoDesktop.Remoting;

namespace ImoutoDesktop.Commands
{
    public class OpenFile : CommandBase
    {
        public OpenFile()
            : base("(.+?)を(開く|表示)")
        {
        }

        private string _path;

        public override bool PreExecute(string input)
        {
            var match = _pattern.Match(input);
            var target = match.Groups[1].Value;
            var directory = ConnectionPool.Connection.CurrentDirectory;

            _path = AbsolutePath(directory, target);
            Parameters = new[] { Escape(_path) };

            return true;
        }

        public override bool Execute(string input, out string result)
        {
            result = null;

            if (ConnectionPool.Connection.Exists(_path) != Exists.File)
            {
                Parameters = new[] { Escape(_path), "not exist" };
                return false;
            }

            if (!ConnectionPool.Connection.ExecuteProcess(_path, string.Empty))
            {
                Parameters = new[] { Escape(_path), "unknown" };
                return false;
            }

            Parameters = new[] { Escape(_path) };

            return true;
        }
    }
}
