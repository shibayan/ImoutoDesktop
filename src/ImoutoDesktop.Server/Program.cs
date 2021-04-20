using System;
using System.Threading.Tasks;

using Grpc.Core;

namespace ImoutoDesktop.Server
{
    static class Program
    {
        static async Task Main()
        {
            var server = new Grpc.Core.Server
            {
                Services = { Remoting.RemoteService.BindService(new RemoteServiceImpl()) },
                Ports = { new ServerPort("0.0.0.0", 1024, ServerCredentials.Insecure) }
            };

            server.Start();

            Console.WriteLine("Started");
            Console.ReadKey();

            await server.ShutdownAsync();
        }
    }
}
