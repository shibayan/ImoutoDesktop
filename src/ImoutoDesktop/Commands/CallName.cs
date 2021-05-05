using ImoutoDesktop.Services;

namespace ImoutoDesktop.Commands
{
    public class CallName : CommandBase
    {
        public CallName(string name, RemoteConnectionManager remoteConnectionManager)
            : base(name, remoteConnectionManager)
        {
        }

        public override Priority Priority => Priority.Lowest;
    }
}
