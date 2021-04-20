namespace ImoutoDesktop.Commands
{
    public class CallName : CommandBase
    {
        public CallName(string name)
            : base(name)
        {
        }

        public override Priority Priority
        {
            get { return Priority.Lowest; }
        }

        public override bool IsExecute(string input)
        {
            return _pattern.IsMatch(input);
        }

        public override bool PreExecute(string input)
        {
            return true;
        }

        public override bool Execute(string input, out string result)
        {
            result = null;
            return true;
        }
    }
}
