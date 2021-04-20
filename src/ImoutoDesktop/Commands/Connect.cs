namespace ImoutoDesktop.Commands
{
    public class Connect : CommandBase
    {
        public Connect()
            : base("接続")
        {
        }

        public override Priority Priority => Priority.Highest;

        public override CommandResult PreExecute(string input)
        {
            return Succeeded(new[] { Settings.Default.ServerAddress });
        }

        public override CommandResult Execute(string input)
        {
            if (!ConnectionPool.IsConnected)
            {
                var ret = ConnectionPool.Connect(Settings.Default.ServerAddress, Settings.Default.PortNumber, Settings.Default.Password);

                if (!ret.HasValue)
                {
                    return Failed();
                }

                if (!ret.Value)
                {
                    return Failed();
                }
            }

            return Succeeded();
        }
    }
}
