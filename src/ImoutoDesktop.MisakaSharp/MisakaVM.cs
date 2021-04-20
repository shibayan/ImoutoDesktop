using System;
using System.IO;

namespace ImoutoDesktop.MisakaSharp
{
    class MisakaVM
    {
        public MisakaVM()
        {
            random = new Random();
            parser = new Parser();
            settings = new Settings();
            functions = new Functions();
            variables = new Variables();
        }

        public void StartTrace()
        {
            parseTracer = new Tracer(Path.Combine(rootDirectory, "misaka_error.txt"), settings.EnableErrorLog);
            runtimeTracer = new Tracer(Path.Combine(rootDirectory, "misaka_debug.txt"), settings.EnableDebugLog);
        }

        public void EndTrace()
        {
            parseTracer.Close();
            runtimeTracer.Close();
        }

        public string ExecFunction(string name)
        {
            if (!functions.IsFunction(name))
            {
                return string.Empty;
            }
            var retval = functions.ExecFunction(this, name);
            return retval.ToString();
        }

        private string rootDirectory;

        public string RootDirectory
        {
            get { return rootDirectory; }
            set { rootDirectory = value; }
        }

        private string[] parameter;

        public string[] Parameter
        {
            get { return parameter; }
            set { parameter = value; }
        }

        private Random random;

        public Random Random
        {
            get { return random; }
        }

        private Parser parser;

        internal Parser Parser
        {
            get { return parser; }
        }

        private Settings settings;

        internal Settings Settings
        {
            get { return settings; }
        }

        private Functions functions;

        internal Functions Functions
        {
            get { return functions; }
        }

        private Variables variables;

        internal Variables Variables
        {
            get { return variables; }
        }

        private Tracer parseTracer;

        internal Tracer ParseTracer
        {
            get { return parseTracer; }
        }

        private Tracer runtimeTracer;

        internal Tracer RuntimeTracer
        {
            get { return runtimeTracer; }
        }
    }
}
