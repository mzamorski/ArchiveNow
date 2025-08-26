using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ArchiveNow.RemoteUpload.Server;

Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureLogging((ctx, logging) =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddEventLog(o => o.SourceName = "ArchiveNow.RemoteUpload");
    })
    .ConfigureServices((ctx, services) =>
    {
        services.Configure<RemoteUploadConfiguration>(
            ctx.Configuration.GetSection("RemoteUpload"));

        services.AddHostedService<RemoteUploadService>();
    })
    .Build()
    .Run();