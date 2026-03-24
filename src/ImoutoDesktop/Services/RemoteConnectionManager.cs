using Grpc.Net.Client;

using ImoutoDesktop.Remoting;

namespace ImoutoDesktop.Services;

public class RemoteConnectionManager
{
    private GrpcChannel? _channel;
    private RemoteService.RemoteServiceClient? _remoteServiceClient;

    public Task ConnectAsync(string host, int port)
    {
        _channel = GrpcChannel.ForAddress($"http://{host}:{port}");

        _remoteServiceClient = new RemoteService.RemoteServiceClient(_channel);

        return Task.CompletedTask;
    }

    public async Task DisconnectAsync()
    {
        if (_channel == null)
        {
            return;
        }

        await _channel.ShutdownAsync();

        _remoteServiceClient = null;
        _channel = null;
    }

    public RemoteService.RemoteServiceClient GetServiceClient()
    {
        return _remoteServiceClient!;
    }
}
