namespace ImoutoDesktop.Commands
{
    public class Connect : CommandBase
    {
        public Connect()
            : base("(接続|切断)")
        {
        }

        public override Priority Priority
        {
            get { return Priority.Highest; }
        }

        public override bool IsExecute(string input)
        {
            return _pattern.IsMatch(input);
        }

        public override bool PreExecute(string input)
        {
            var match = _pattern.Match(input);
            var subtext = match.Groups[1].Value;

            switch (subtext)
            {
                case "接続":
                    EventID = "Logined";
                    Parameters = new[] { Settings.Default.ServerAddress };
                    break;
                case "切断":
                    EventID = "Disconnected";
                    break;
            }

            return true;
        }

        public override bool Execute(string input, out string result)
        {
            result = null;
            EventID = null;

            var match = _pattern.Match(input);
            var subtext = match.Groups[1].Value;

            switch (subtext)
            {
                case "接続":
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
                    break;
                case "切断":
                    ConnectionPool.Disconnect();
                    break;
            }

            return true;
        }
    }
}
