using System;
using System.IO;

namespace ImoutoDesktop.Scripting
{
    class ExecutionContext
    {
        public ExecutionContext()
        {
            Random = new Random();
            Parser = new Parser();
            Settings = new Settings();
            Functions = new Functions();
            Variables = new Variables();
        }

        public void StartTrace()
        {
            ParseTracer = new Tracer(Path.Combine(RootDirectory, "misaka_error.txt"), Settings.EnableErrorLog);
            RuntimeTracer = new Tracer(Path.Combine(RootDirectory, "misaka_debug.txt"), Settings.EnableDebugLog);
        }

        public void EndTrace()
        {
            ParseTracer.Close();
            RuntimeTracer.Close();
        }

        public string ExecFunction(string name)
        {
            if (!Functions.IsFunction(name))
            {
                return string.Empty;
            }
            var retval = Functions.ExecFunction(this, name);
            return retval.ToString();
        }

        public string RootDirectory { get; set; }

        public string[] Parameter { get; set; }

        public Random Random { get; }

        internal Parser Parser { get; }

        internal Settings Settings { get; }

        internal Functions Functions { get; }

        internal Variables Variables { get; }

        internal Tracer ParseTracer { get; private set; }

        internal Tracer RuntimeTracer { get; private set; }
    }
}
