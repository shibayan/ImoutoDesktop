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
                Ports = { new ServerPort("localhost", 5000, ServerCredentials.Insecure) }
            };

            server.Start();

            Console.ReadKey();

            await server.ShutdownAsync();
        }
    }
}
