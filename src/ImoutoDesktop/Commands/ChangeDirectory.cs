using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Google.Protobuf.WellKnownTypes;

using ImoutoDesktop.Models;
using ImoutoDesktop.Remoting;
using ImoutoDesktop.Services;

using Enum = System.Enum;

namespace ImoutoDesktop.Commands
{
    public class ChangeDirectory : RemoteCommandBase
    {
        public ChangeDirectory(RemoteConnectionManager remoteConnectionManager)
            : base(@"^(.+?)[へに]移動", remoteConnectionManager)
        {
        }

        private string _directory;

        private static readonly Dictionary<string, SpecialDirectory> _table = new()
        {
            { "デスクトップ", SpecialDirectory.Desktop },
            { "ミュージック", SpecialDirectory.MyMusic },
            { "マイミュージック", SpecialDirectory.MyMusic },
            { "ドキュメント", SpecialDirectory.MyDocuments },
            { "マイドキュメント", SpecialDirectory.MyDocuments },
            { "ピクチャ", SpecialDirectory.MyPictures },
            { "マイピクチャ", SpecialDirectory.MyPictures }
        };

        protected override async Task<CommandResult> PreExecuteCore(string input)
        {
            var match = Pattern.Match(input);
            var target = match.Groups[1].Value;

            var serviceClient = RemoteConnectionManager.GetServiceClient();

            _directory = (await serviceClient.GetCurrentDirectoryAsync(new Empty())).Path;

            if (_table.TryGetValue(target, out var specialDirectory))
            {
                var response = await serviceClient.GetDirectoryPathAsync(new GetDirectoryPathRequest { SpecialDirectory = specialDirectory });

                _directory = response.Path;
            }
            else
            {
                _directory = AbsolutePath(_directory, target);
            }

            var type = DirectoryType.Empty;

            if (Settings.Default.AutoDetectDirectoryType)
            {
                var typeResponse = await serviceClient.GetDirectoryTypeAsync(new GetDirectoryTypeRequest { Path = _directory });

                type = typeResponse.DirectoryType;
            }

            return Succeeded(new[] { Escape(_directory), Enum.GetName(typeof(DirectoryType), type) });
        }

        protected override async Task<CommandResult> ExecuteCore(string input)
        {
            var serviceClient = RemoteConnectionManager.GetServiceClient();

            var existResponse = serviceClient.Exists(new ExistsRequest { Path = _directory });

            if (!existResponse.Exists)
            {
                return Failed(new[] { Escape(_directory), "not exist" });
            }

            try
            {
                await serviceClient.SetCurrentDirectoryAsync(new SetCurrentDirectoryRequest { Path = _directory });

                if (Settings.Default.ShowFileList)
                {
                    var files = await serviceClient.GrepAsync(new GrepRequest { Path = _directory, SearchPattern = "*", Kind = Kind.File });
                    var directories = await serviceClient.GrepAsync(new GrepRequest { Path = _directory, SearchPattern = "*", Kind = Kind.Directory });

                    var ret = new StringBuilder();

                    ret.Append(@"■ディレクトリ\n");

                    foreach (var item in directories.Files)
                    {
                        ret.AppendFormat(@"{0}\n", Path.GetFileName(item));
                    }

                    ret.Append(@"■ファイル\n");

                    foreach (var item in files.Files)
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
