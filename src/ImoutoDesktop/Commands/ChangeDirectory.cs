using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ImoutoDesktop.Remoting;

namespace ImoutoDesktop.Commands
{
    public class ChangeDirectory : CommandBase
    {
        public ChangeDirectory()
            : base(@"^(.+?)[へに]移動")
        {
        }

        private string _directory;

        private static readonly Dictionary<string, Environment.SpecialFolder> _table = new()
        {
            { "デスクトップ", Environment.SpecialFolder.Desktop },
            { "ミュージック", Environment.SpecialFolder.MyMusic },
            { "マイミュージック", Environment.SpecialFolder.MyMusic },
            { "ドキュメント", Environment.SpecialFolder.MyDocuments },
            { "マイドキュメント", Environment.SpecialFolder.MyDocuments },
            { "ピクチャ", Environment.SpecialFolder.MyPictures },
            { "マイピクチャ", Environment.SpecialFolder.MyPictures }
        };

        public override CommandResult PreExecute(string input)
        {
            var match = Pattern.Match(input);
            var target = match.Groups[1].Value;

            _directory = ConnectionPool.Connection.CurrentDirectory;

            _directory = _table.ContainsKey(target) ? ConnectionPool.Connection.GetFolderPath(_table[target]) : AbsolutePath(_directory, target);

            if (!_directory.EndsWith(@"\"))
            {
                _directory += @"\";
            }

            var type = DirectoryType.None;

            if (Settings.Default.AutoDetectDirectoryType)
            {
                type = ConnectionPool.Connection.GetDirectoryType(_directory);
            }

            return Succeeded(Escape(_directory), Enum.GetName(typeof(DirectoryType), type));
        }

        public override CommandResult Execute(string input)
        {
            if (ConnectionPool.Connection.Exists(_directory) != Exists.Directory)
            {
                return Failed(new[] {Escape(_directory), "not exist"});
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

                    return Succeeded(ret.ToString());
                }

                return Succeeded();
            }
            catch
            {
                return Failed(new[] { Escape(_directory), "unknown" });
            }
        }
    }
}
