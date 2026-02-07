using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ArchiveNow.RemoteUpload.Server;

Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        // Keep defaults (appsettings, env vars, cmdline), ale dołóż nasz globalny plik:
        var sourcesBefore = config.Sources.Count;

        // Candidate locations for ArchiveNow.conf
        string? envPath = Environment.GetEnvironmentVariable("ARCHIVENOW_CONFIG");
        var candidates = new[]
        {
            envPath,
            Path.Combine(AppContext.BaseDirectory, "ArchiveNow.conf"),
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "ArchiveNow", "ArchiveNow.conf"
            ),
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ArchiveNow", "ArchiveNow.conf"
            )
        }
        .Where(p => !string.IsNullOrWhiteSpace(p))
        .Distinct()
        .ToList();

        string? picked = candidates.FirstOrDefault(File.Exists);
        if (picked is null)
        {
            // Optional: fail fast to uniknąć cichego startu bez konfiguracji
            throw new FileNotFoundException(
                "ArchiveNow.conf not found. Set ARCHIVENOW_CONFIG or place the file in BaseDirectory or ProgramData\\ArchiveNow."
            );
        }

        // Assuming JSON format; set reloadOnChange for live updates
        config.AddJsonFile(picked, optional: false, reloadOnChange: true);

        // Store the selected path for logging
        hostingContext.Properties["ArchiveNow:ConfigPath"] = picked;
    })
    .ConfigureLogging((ctx, logging) =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddEventLog(o =>
        {
            // Name of the source we registered in Windows
            o.SourceName = "ArchiveNow.Server";

            // Ensure it points to the correct Log (e.g., Application)
            o.LogName = "Application";
        });

        // Optional: log where the config was loaded from
        if (ctx.Properties.TryGetValue("ArchiveNow:ConfigPath", out var pathObj) && pathObj is string path)
        {
            var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
            var logger = loggerFactory.CreateLogger("Startup");
            logger.LogInformation("Loaded global configuration from: {ConfigPath}", path);
        }
    })
    .ConfigureServices((ctx, services) =>
    {
        services.AddOptions();
        services.AddHostedService<RemoteUploadService>();
        services.AddRemoteUploadConfigWithEnvExpansion();
    })
    .Build()
    .Run();

public static class RemoteUploadConfigurationExtensions
{
    public static IServiceCollection AddRemoteUploadConfigWithEnvExpansion(this IServiceCollection services)
    {
        services.AddOptions<RemoteUploadConfiguration>()
            .Configure<IConfiguration>((opts, cfg) =>
            {
                var section = cfg.GetSection("RemoteUploadServer");
                if (section.Exists())
                {
                    section.Bind(opts);
                }
                else
                {
                    cfg.Bind(opts);
                }
            })
            .PostConfigure(options =>
            {
                // Expand env vars just for the needed fields
                if (!string.IsNullOrEmpty(options.UploadsDirectory))
                {
                    options.UploadsDirectory =
                        Environment.ExpandEnvironmentVariables(options.UploadsDirectory);
                }
            });

        return services;
    }
}