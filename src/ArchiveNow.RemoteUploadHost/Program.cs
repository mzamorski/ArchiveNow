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
        logging.AddEventLog(o => o.SourceName = "ArchiveNow.RemoteUploadServer");

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
        // Flexible binding:
        // If config has "RemoteUpload" section -> bind from that section,
        // else bind from root.
        IConfiguration config = ctx.Configuration;
        var section = config.GetSection("RemoteUploadServer");

        if (section.Exists())
        {
            services.Configure<RemoteUploadConfiguration>(section);
        }
        else
        {
            services.Configure<RemoteUploadConfiguration>(config);
        }

        services.AddHostedService<RemoteUploadService>();
    })
    .Build()
    .Run();
