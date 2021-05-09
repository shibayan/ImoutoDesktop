using System.Threading.Tasks;

using ImoutoDesktop.Services;

namespace ImoutoDesktop.Commands
{
    public class Disconnect : RemoteCommandBase
    {
        public Disconnect(RemoteConnectionManager remoteConnectionManager)
            : base("切断", remoteConnectionManager)
        {
        }

        public override Priority Priority => Priority.Highest;

        protected override async Task<CommandResult> ExecuteCore(string input)
        {
            await RemoteConnectionManager.DisconnectAsync();

            return Succeeded();
        }
    }
}
