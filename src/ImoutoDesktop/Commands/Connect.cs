using System.Threading.Tasks;

using ImoutoDesktop.Models;
using ImoutoDesktop.Services;

namespace ImoutoDesktop.Commands
{
    public class Connect : RemoteCommandBase
    {
        public Connect(RemoteConnectionManager remoteConnectionManager)
            : base("接続", remoteConnectionManager)
        {
        }

        public override Priority Priority => Priority.Highest;

        public override Task<CommandResult> PreExecute(string input)
        {
            return Task.FromResult(Succeeded(new[] { Settings.Default.ServerAddress }));
        }

        public override async Task<CommandResult> Execute(string input)
        {
            if (RemoteConnectionManager.GetServiceClient() == null)
            {
                await RemoteConnectionManager.ConnectAsync(Settings.Default.ServerAddress, Settings.Default.PortNumber);
            }

            return Succeeded(new[] { Settings.Default.ServerAddress });
        }
    }
}
