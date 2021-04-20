using System.Threading.Tasks;

using Google.Protobuf.WellKnownTypes;

using ImoutoDesktop.Remoting;

namespace ImoutoDesktop.Commands
{
    public class DeleteFile : CommandBase
    {
        public DeleteFile(RemoteConnectionManager remoteConnectionManager)
            : base(@"(.+?)を削除", remoteConnectionManager)
        {
        }

        private string _path;

        public override async Task<CommandResult> PreExecute(string input)
        {
            var serviceClient = RemoteConnectionManager.GetServiceClient();

            var match = Pattern.Match(input);
            var target = match.Groups[1].Value;

            var directory = (await serviceClient.GetCurrentDirectoryAsync(new Empty())).Path;

            _path = AbsolutePath(directory, target);

            return Succeeded(new[] { Escape(_path) });
        }

        public override async Task<CommandResult> Execute(string input)
        {
            var serviceClient = RemoteConnectionManager.GetServiceClient();

            var existsResponse = await serviceClient.ExistsAsync(new ExistsRequest { Path = _path });

            if (!existsResponse.Exists || existsResponse.Kind != Kind.File)
            {
                return Failed(new[] { Escape(_path), "not exist" });
            }

            var deleteResponse = await serviceClient.DeleteAsync(new DeleteRequest { Path = _path });

            if (!deleteResponse.Succeeded)
            {
                return Failed(new[] { Escape(_path), "unknown" });
            }

            return Succeeded(new[] { Escape(_path) });
        }
    }
}
