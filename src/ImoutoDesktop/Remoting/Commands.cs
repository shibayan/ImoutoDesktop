using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows;

using ImoutoDesktop.Commands;

namespace ImoutoDesktop.Remoting
{
    public abstract class CommandBase : ICommand
    {
        public CommandBase(string pattern)
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
            return _pattern.IsMatch(input) && ConnectionPool.IsConnected;
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
                Uri u1 = new Uri(directory.EndsWith(@"\") ? directory : directory + @"\");
                Uri u2 = new Uri(u1, path);
                directory = u2.LocalPath;
            }
            return directory;
        }
    }

    public class Connect : CommandBase
    {
        public Connect()
            : base("(接続|切断)")
        {
        }

        public override Priority Priority
        {
            get { return Priority.Highest; }
        }

        public override bool IsExecute(string input)
        {
            return _pattern.IsMatch(input);
        }

        public override bool PreExecute(string input)
        {
            Match match = _pattern.Match(input);
            string subtext = match.Groups[1].Value;
            switch (subtext)
            {
                case "接続":
                    EventID = "Logined";
                    Parameters = new[] { Settings.Default.ServerAddress };
                    break;
                case "切断":
                    EventID = "Disconnected";
                    break;
            }
            return true;
        }

        public override bool Execute(string input, out string result)
        {
            result = null;
            EventID = null;
            Match match = _pattern.Match(input);
            string subtext = match.Groups[1].Value;
            switch (subtext)
            {
                case "接続":
                    if (!ConnectionPool.IsConnected)
                    {
                        var ret = ConnectionPool.Connect(Settings.Default.ServerAddress, Settings.Default.PortNumber, Settings.Default.Password);
                        if (!ret.HasValue)
                        {
                            return false;
                        }
                        else if (!ret.Value)
                        {
                            EventID = "IncorrectPassword";
                        }
                    }
                    break;
                case "切断":
                    ConnectionPool.Disconnect();
                    break;
            }
            return true;
        }
    }

    public class ChangeDirectory : CommandBase
    {
        public ChangeDirectory()
            : base(@"^(.+?)[へに]移動")
        {
            _table.Add("デスクトップ", Environment.SpecialFolder.Desktop);
            _table.Add("ミュージック", Environment.SpecialFolder.MyMusic);
            _table.Add("マイミュージック", Environment.SpecialFolder.MyMusic);
            _table.Add("ドキュメント", Environment.SpecialFolder.MyDocuments);
            _table.Add("マイドキュメント", Environment.SpecialFolder.MyDocuments);
            _table.Add("ピクチャ", Environment.SpecialFolder.MyPictures);
            _table.Add("マイピクチャ", Environment.SpecialFolder.MyPictures);
        }

        private string _directory;

        private static readonly Dictionary<string, Environment.SpecialFolder> _table = new Dictionary<string, Environment.SpecialFolder>();

        public override bool PreExecute(string input)
        {
            var match = _pattern.Match(input);
            var target = match.Groups[1].Value;
            _directory = ConnectionPool.Connection.CurrentDirectory;
            if (_table.ContainsKey(target))
            {
                _directory = ConnectionPool.Connection.GetFolderPath(_table[target]);
            }
            else
            {
                _directory = AbsolutePath(_directory, target);
            }
            if (!_directory.EndsWith(@"\"))
            {
                _directory += @"\";
            }
            var type = DirectoryType.None;
            if (Settings.Default.AutoDetectDirectoryType)
            {
                type = ConnectionPool.Connection.GetDirectoryType(_directory);
            }
            Parameters = new[] { Escape(_directory), Enum.GetName(typeof(DirectoryType), type) };
            return true;
        }

        public override bool Execute(string input, out string result)
        {
            result = null;
            if (ConnectionPool.Connection.Exists(_directory) != Exists.Directory)
            {
                Parameters = new[] { Escape(_directory), "not exist" };
                return false;
            }
            try
            {
                ConnectionPool.Connection.CurrentDirectory = _directory;
                if (Settings.Default.ShowFileList)
                {
                    var files = ConnectionPool.Connection.GetFiles(_directory, "*");
                    var directories = ConnectionPool.Connection.GetDirectories(_directory, "*");
                    var ret = new StringBuilder();
                    ret.Append(@"■ディレクトリ\n");
                    foreach (var item in directories)
                    {
                        ret.AppendFormat(@"{0}\n", Path.GetFileName(item));
                    }
                    ret.Append(@"■ファイル\n");
                    foreach (var item in files)
                    {
                        ret.AppendFormat(@"{0}\n", Path.GetFileName(item));
                    }
                    result = ret.ToString();
                }
            }
            catch
            {
                Parameters = new[] { Escape(_directory), "unknown" };
                return false;
            }
            return true;
        }
    }

    public class DosCommand : CommandBase
    {
        public DosCommand()
            : base(@".+")
        {
        }

        private string _directory;

        private static readonly string[] _allow = new string[]
        {
            "dir", "ver", "xcopy", "mkdir", "rmdir", "copy", "del", "move", "ren", "cd", "type", "ls", "chdir", "rm", "cp", "cls", "start"
        };

        private static readonly Dictionary<string, string> _replace = new Dictionary<string, string>
        {
            { "chdir", "cd" }, { "ls", "dir" }, { "rm", "del" }, { "cp", "copy" }
        };

        public override Priority Priority
        {
            get { return Priority.BelowNormal; }
        }

        public override bool IsExecute(string input)
        {
            return Array.Exists(_allow, p => input == p || input.StartsWith(p + " ")) && ConnectionPool.IsConnected;
        }

        public override bool PreExecute(string input)
        {
            EventID = null;
            if (input.StartsWith("cd"))
            {
                EventID = "ChangeDirectory";
                input = input.Trim();
                if (input.Length < 3)
                {
                    Parameters = new[] { Escape(input), "unknown" };
                    return false;
                }
                _directory = AbsolutePath(ConnectionPool.Connection.CurrentDirectory, input.Substring(2).Trim());
                if (!_directory.EndsWith(@"\"))
                {
                    _directory += @"\";
                }
                var type = DirectoryType.None;
                if (Settings.Default.AutoDetectDirectoryType)
                {
                    type = ConnectionPool.Connection.GetDirectoryType(_directory);
                }
                Parameters = new[] { Escape(_directory), Enum.GetName(typeof(DirectoryType), type) };
                return true;
            }
            Parameters = new[] { Escape(input) };
            return true;
        }

        public override bool Execute(string input, out string result)
        {
            EventID = null;
            foreach (var item in _replace)
            {
                if (input.StartsWith(item.Key + " "))
                {
                    input = item.Key + input.Substring(item.Key.Length);
                    break;
                }
            }
            if (input.StartsWith("cd"))
            {
                result = null;
                EventID = "ChangeDirectory";
                if (ConnectionPool.Connection.Exists(_directory) != Exists.Directory)
                {
                    Parameters = new[] { Escape(_directory), "not exist" };
                    return false;
                }
                try
                {
                    ConnectionPool.Connection.CurrentDirectory = _directory;
                }
                catch
                {
                    Parameters = new[] { Escape(_directory), "unknown" };
                    return false;
                }
                return true;
            }
            else if (input.StartsWith("cls"))
            {
                result = null;
                return true;
            }
            result = ConnectionPool.Connection.ExecuteCommand(input);
            return true;
        }
    }

    public class ExecuteFile : CommandBase
    {
        public ExecuteFile()
            : base("(.+?)を実行")
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

    public class CopyFile : CommandBase
    {
        public CopyFile()
            : base("")
        {
        }

        public override bool PreExecute(string input)
        {
            throw new NotImplementedException();
        }

        public override bool Execute(string input, out string result)
        {
            throw new NotImplementedException();
        }
    }

    public class MoveFile : CommandBase
    {
        public MoveFile()
            : base("")
        {
        }

        public override bool PreExecute(string input)
        {
            throw new NotImplementedException();
        }

        public override bool Execute(string input, out string result)
        {
            throw new NotImplementedException();
        }
    }

    public class DeleteFile : CommandBase
    {
        public DeleteFile()
            : base(@"(.+?)を削除")
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
            if (!ConnectionPool.Connection.DeleteFile(_path))
            {
                Parameters = new[] { Escape(_path), "unknown" };
                return false;
            }
            Parameters = new[] { Escape(_path) };
            return true;
        }
    }

    public class DownloadFile : CommandBase
    {
        public DownloadFile()
            : base(@"(.+?)を(受信|ダウンロード)")
        {
        }

        public override bool PreExecute(string input)
        {
            throw new NotImplementedException();
        }

        public override bool Execute(string input, out string result)
        {
            throw new NotImplementedException();
        }
    }

    public class ScreenShot : CommandBase
    {
        public ScreenShot()
            : base("スクリーンショット")
        {
        }

        public override bool PreExecute(string input)
        {
            return true;
        }

        public override bool Execute(string input, out string result)
        {
            try
            {
                string path = Path.Combine(((App)Application.Current).RootDirectory, string.Format(@"temp\{0}.png", Path.GetRandomFileName()));
                Stream stream = ConnectionPool.Connection.GetScreenshot(480);
                Bitmap bitmap = new Bitmap(stream);
                bitmap.Save(path);
                bitmap.Dispose();
                result = string.Format(@"\_i[{0}]", path);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }

    public class CallName : CommandBase
    {
        public CallName(string name)
            : base(name)
        {
        }

        public override Priority Priority
        {
            get { return Priority.Lowest; }
        }

        public override bool IsExecute(string input)
        {
            return _pattern.IsMatch(input);
        }

        public override bool PreExecute(string input)
        {
            return true;
        }

        public override bool Execute(string input, out string result)
        {
            result = null;
            return true;
        }
    }
}
