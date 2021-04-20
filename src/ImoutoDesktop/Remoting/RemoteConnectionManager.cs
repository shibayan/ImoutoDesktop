using System.Threading.Tasks;

using Grpc.Core;

namespace ImoutoDesktop.Remoting
{
    public class RemoteConnectionManager
    {
        private Channel _channel;
        private RemoteService.RemoteServiceClient _remoteServiceClient;

        public async Task ConnectAsync(string host, int port)
        {
            _channel = new Channel(host, port, ChannelCredentials.Insecure);

            _remoteServiceClient = new RemoteService.RemoteServiceClient(_channel);

            await _channel.ConnectAsync();
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
            return _remoteServiceClient;
        }
    }
}
