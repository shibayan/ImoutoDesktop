using ImoutoDesktop.Remoting;

namespace ImoutoDesktop.Commands
{
    public class ExecuteFile : CommandBase
    {
        public ExecuteFile()
            : base("(.+?)を実行")
        {
        }

        private string _path;

        public override CommandResult PreExecute(string input)
        {
            var match = Pattern.Match(input);
            var target = match.Groups[1].Value;
            var directory = ConnectionPool.Connection.CurrentDirectory;

            _path = AbsolutePath(directory, target);

            return Succeeded(new[] { Escape(_path) });
        }

        public override CommandResult Execute(string input)
        {
            if (ConnectionPool.Connection.Exists(_path) != Exists.File)
            {
                return Failed(new[] { Escape(_path), "not exist" });
            }

            if (!ConnectionPool.Connection.ExecuteProcess(_path, string.Empty))
            {
                return Failed(new[] { Escape(_path), "unknown" });
            }

            return Succeeded(new[] { Escape(_path) });
        }
    }
}
