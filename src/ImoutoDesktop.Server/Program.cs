using System.Net;
using System.Net.Sockets;

using ImoutoDesktop.Server;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

var port = 1024;
var ipAddress = (await Dns.GetHostAddressesAsync(Dns.GetHostName())).First(x => x.AddressFamily == AddressFamily.InterNetwork);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(port, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

var app = builder.Build();

if (Environment.OSVersion.Platform == PlatformID.Win32NT)
{
    app.MapGrpcService<WindowsRemoteServiceImpl>();
}
else
{
    app.MapGrpcService<UnixRemoteServiceImpl>();
}

Console.WriteLine($"Started - {ipAddress}:{port}");

await app.RunAsync();
