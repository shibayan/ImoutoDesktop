using System.Threading.Tasks;

using Google.Protobuf.WellKnownTypes;

using ImoutoDesktop.Remoting;
using ImoutoDesktop.Services;

namespace ImoutoDesktop.Commands;

public class OpenFile : RemoteCommandBase
{
    public OpenFile(RemoteConnectionManager remoteConnectionManager)
        : base("(.+?)を(開く|開いて|表示)", remoteConnectionManager)
    {
    }

    private string _path;

    protected override async Task<CommandResult> PreExecuteCore(string input)
    {
        var serviceClient = RemoteConnectionManager.GetServiceClient();

        var match = Pattern.Match(input);
        var target = match.Groups[1].Value;

        var directory = (await serviceClient.GetCurrentDirectoryAsync(new Empty())).Path;

        _path = AbsolutePath(directory, target);

        return Succeeded(new[] { Escape(_path) });
    }

    protected override async Task<CommandResult> ExecuteCore(string input)
    {
        var serviceClient = RemoteConnectionManager.GetServiceClient();

        var existsResponse = await serviceClient.ExistsAsync(new ExistsRequest { Path = _path });

        if (!existsResponse.Exists || existsResponse.Kind != Kind.File)
        {
            return Failed(new[] { Escape(_path), "not exist" });
        }

        var executeResponse = await serviceClient.ExecuteAsync(new ExecuteRequest { Path = _path });

        if (!executeResponse.Succeeded)
        {
            return Failed(new[] { Escape(_path), "unknown" });
        }

        return Succeeded(new[] { Escape(_path) });
    }
}
