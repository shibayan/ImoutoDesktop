namespace ImoutoDesktop.Commands
{
    public class Disconnect : CommandBase
    {
        public Disconnect()
            : base("切断")
        {
        }

        public override Priority Priority => Priority.Highest;

        public override CommandResult Execute(string input)
        {
            ConnectionPool.Disconnect();

            return Succeeded();
        }
    }
}
