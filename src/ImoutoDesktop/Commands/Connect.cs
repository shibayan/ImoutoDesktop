namespace ImoutoDesktop.Commands
{
    public class Connect : CommandBase
    {
        public Connect()
            : base("接続")
        {
        }

        public override Priority Priority
        {
            get { return Priority.Highest; }
        }

        public override bool PreExecute(string input)
        {
            EventID = "Logined";
            Parameters = new[] { Settings.Default.ServerAddress };

            return true;
        }

        public override bool Execute(string input, out string result)
        {
            result = null;
            EventID = null;

            if (!ConnectionPool.IsConnected)
            {
                var ret = ConnectionPool.Connect(Settings.Default.ServerAddress, Settings.Default.PortNumber, Settings.Default.Password);

                if (!ret.HasValue)
                {
                    return false;
                }

                if (!ret.Value)
                {
                    EventID = "IncorrectPassword";
                }
            }

            return true;
        }
    }
}
