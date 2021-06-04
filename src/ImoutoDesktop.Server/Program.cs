using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using Grpc.Core;

namespace ImoutoDesktop.Server
{
    static class Program
    {
        static async Task Main()
        {
            var port = 1024;
            var ipAddress = (await Dns.GetHostAddressesAsync(Dns.GetHostName())).First(x => x.AddressFamily == AddressFamily.InterNetwork);

            var serviceImpl = default(RemoteServiceImpl);

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                serviceImpl = new WindowsRemoteServiceImpl();
            }
            else
            {
                serviceImpl = new UnixRemoteServiceImpl();
            }

            var server = new Grpc.Core.Server
            {
                Services = { Remoting.RemoteService.BindService(serviceImpl) },
                Ports = { new ServerPort("0.0.0.0", port, ServerCredentials.Insecure) }
            };

            server.Start();

            Console.WriteLine($"Started - {ipAddress}:{port}");
            Console.ReadKey();

            await server.ShutdownAsync();
        }
    }
}
