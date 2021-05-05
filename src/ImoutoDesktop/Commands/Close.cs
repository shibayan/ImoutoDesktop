using ImoutoDesktop.Services;

namespace ImoutoDesktop.Commands
{
    public class Close : CommandBase
    {
        public Close(RemoteConnectionManager remoteConnectionManager)
            : base("終了", remoteConnectionManager)
        {
        }

        public override Priority Priority => Priority.Lowest;
    }
}
