using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using ImoutoDesktop.Remoting;

namespace ImoutoDesktop.Server
{
    public abstract class RemoteServiceImpl : RemoteService.RemoteServiceBase
    {
        public override Task<GenericResponse> Login(LoginRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GenericResponse { Succeeded = true });
        }

        public override Task<GenericResponse> Heartbeat(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new GenericResponse { Succeeded = true });
        }

        public override Task<GrepResponse> Grep(GrepRequest request, ServerCallContext context)
        {
            var response = new GrepResponse();

            if (request.Kind == Kind.File)
            {
                var files = Directory.GetFiles(request.Path, request.SearchPattern);

                response.Files.AddRange(files);
            }
            else if (request.Kind == Kind.Directory)
            {
                var directories = Directory.GetDirectories(request.Path, request.SearchPattern);

                response.Files.AddRange(directories);
            }

            return Task.FromResult(response);
        }

        public override Task<ExistsResponse> Exists(ExistsRequest request, ServerCallContext context)
        {
            if (File.Exists(request.Path))
            {
                return Task.FromResult(new ExistsResponse { Exists = true, Kind = Kind.File });
            }

            if (Directory.Exists(request.Path))
            {
                return Task.FromResult(new ExistsResponse { Exists = true, Kind = Kind.Directory });
            }

            return Task.FromResult(new ExistsResponse { Exists = false });
        }

        public override Task<GenericResponse> Delete(DeleteRequest request, ServerCallContext context)
        {
            if (!File.Exists(request.Path))
            {
                return Task.FromResult(new GenericResponse { Succeeded = false });
            }

            File.Delete(request.Path);

            return Task.FromResult(new GenericResponse { Succeeded = true });
        }

        public override Task<GenericResponse> Execute(ExecuteRequest request, ServerCallContext context)
        {
            Process.Start(request.Path);

            return Task.FromResult(new GenericResponse { Succeeded = true });
        }

        public override Task<GetDirectoryPathResponse> GetDirectoryPath(GetDirectoryPathRequest request, ServerCallContext context)
        {
            var path = request.SpecialDirectory switch
            {
                SpecialDirectory.Desktop => Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                SpecialDirectory.MyPictures => Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                SpecialDirectory.MyVideos => Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                SpecialDirectory.MyMusic => Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                SpecialDirectory.MyDocuments => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                _ => throw new ArgumentOutOfRangeException()
            };

            return Task.FromResult(new GetDirectoryPathResponse { Path = path });
        }

        public override Task<GetDirectoryTypeResponse> GetDirectoryType(GetDirectoryTypeRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GetDirectoryTypeResponse { DirectoryType = DirectoryType.Mixed });
        }

        public override Task<GetCurrentDirectoryResponse> GetCurrentDirectory(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new GetCurrentDirectoryResponse { Path = Environment.CurrentDirectory });
        }

        public override Task<Empty> SetCurrentDirectory(SetCurrentDirectoryRequest request, ServerCallContext context)
        {
            Environment.CurrentDirectory = request.Path;

            return Task.FromResult(new Empty());
        }
    }
}
