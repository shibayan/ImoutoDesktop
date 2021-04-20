namespace ImoutoDesktop.Commands
{
    public class Disconnect : CommandBase
    {
        public Disconnect()
            : base("切断")
        {
        }

        public override Priority Priority
        {
            get { return Priority.Highest; }
        }

        public override bool PreExecute(string input)
        {
            return true;
        }

        public override bool Execute(string input, out string result)
        {
            result = null;
            EventID = null;

            ConnectionPool.Disconnect();

            return true;
        }
    }
}
