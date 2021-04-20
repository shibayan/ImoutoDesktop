namespace ImoutoDesktop.Commands
{
    public class CallName : CommandBase
    {
        public CallName(string name)
            : base(name)
        {
        }

        public override Priority Priority => Priority.Lowest;
    }
}
