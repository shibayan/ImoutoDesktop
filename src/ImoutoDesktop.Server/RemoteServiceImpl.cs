using System.Threading.Tasks;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using ImoutoDesktop.Remoting;

namespace ImoutoDesktop.Server
{
    public class RemoteServiceImpl : Remoting.RemoteService.RemoteServiceBase
    {
        public override async Task<GenericResponse> Login(LoginRequest request, ServerCallContext context)
        {
            return new GenericResponse { Succeeded = true };
        }

        public override Task<GenericResponse> Heartbeat(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new GenericResponse { Succeeded = true });
        }

        public override async Task<GrepResponse> Grep(GrepRequest request, ServerCallContext context)
        {
            return await base.Grep(request, context);
        }

        public override async Task<ExistsResponse> Exists(ExistsRequest request, ServerCallContext context)
        {
            return await base.Exists(request, context);
        }

        public override async Task<GenericResponse> Delete(DeleteRequest request, ServerCallContext context)
        {
            return await base.Delete(request, context);
        }
    }
}
