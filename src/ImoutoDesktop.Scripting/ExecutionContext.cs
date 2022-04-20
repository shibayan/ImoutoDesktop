using System;
using System.IO;

namespace ImoutoDesktop.Scripting;

internal class ExecutionContext
{
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

    public Random Random { get; } = new();

    internal Parser Parser { get; } = new();

    internal Settings Settings { get; } = new();

    internal Functions Functions { get; } = new();

    internal Variables Variables { get; } = new();

    internal Tracer ParseTracer { get; private set; }

    internal Tracer RuntimeTracer { get; private set; }
}
