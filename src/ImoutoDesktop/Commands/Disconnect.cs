using System.Threading.Tasks;

using ImoutoDesktop.Remoting;
using ImoutoDesktop.Services;

namespace ImoutoDesktop.Commands
{
    public class Disconnect : CommandBase
    {
        public Disconnect(RemoteConnectionManager remoteConnectionManager)
            : base("切断", remoteConnectionManager)
        {
        }

        public override Priority Priority => Priority.Highest;

        public override async Task<CommandResult> Execute(string input)
        {
            await RemoteConnectionManager.DisconnectAsync();

            return Succeeded();
        }
    }
}
