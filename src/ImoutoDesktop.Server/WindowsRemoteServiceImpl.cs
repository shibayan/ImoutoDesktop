using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Grpc.Core;

using ImoutoDesktop.Remoting;

namespace ImoutoDesktop.Server;

public class WindowsRemoteServiceImpl : RemoteServiceImpl
{
    public override Task<RunShellResponse> RunShell(RunShellRequest request, ServerCallContext context)
    {
        var psi = new ProcessStartInfo
        {
            FileName = Environment.GetEnvironmentVariable("ComSpec"),
            RedirectStandardInput = false,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = Environment.CurrentDirectory,
            Arguments = $"/c {request.Command}"
        };

        var process = Process.Start(psi);
        var result = process.StandardOutput.ReadToEnd();

        process.WaitForExit();

        return Task.FromResult(new RunShellResponse { Result = result });
    }
}
