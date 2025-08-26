using System.Net;
using System.Text;
using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ArchiveNow.RemoteUpload.Server;

public sealed class RemoteUploadService : BackgroundService, IDisposable
{
    private readonly ILogger<RemoteUploadService> _logger;
    private readonly RemoteUploadConfiguration _config;
    private readonly HttpListener _listener = new();
    private readonly ConcurrentBag<Task> _inFlight = new();

    public RemoteUploadService(
        IOptions<RemoteUploadConfiguration> options,
        ILogger<RemoteUploadService> logger)
    {
        _logger = logger;
        _config = options.Value;

        if (string.IsNullOrWhiteSpace(_config.UploadsDirectory))
        {
            throw new ArgumentException("Uploads directory must be provided", nameof(_config.UploadsDirectory));
        }

        Directory.CreateDirectory(_config.UploadsDirectory);

        // Listen on all interfaces (URL ACL required for the service account)
        _listener.Prefixes.Add($"http://+:{_config.Port}/");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting HTTP listener on port {Port}", _config.Port);

        try
        {
            _listener.Start();
        }
        catch (HttpListenerException ex)
        {
            _logger.LogError(ex,
                "Failed to start HttpListener on port {Port}. If running as a service, you likely need a URLACL for the service account.",
                _config.Port);
            throw;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            HttpListenerContext? ctx = null;
            try
            {
                ctx = await _listener.GetContextAsync().ConfigureAwait(false);
            }
            catch (HttpListenerException) when (!_listener.IsListening || stoppingToken.IsCancellationRequested)
            {
                break; // stopped by StopAsync()
            }
            catch (ObjectDisposedException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Accept failed");
                continue;
            }

            var task = HandleContextAsync(ctx, stoppingToken);

            _inFlight.Add(task);

            _ = task.ContinueWith(_ => { }, TaskScheduler.Default);
        }

        _logger.LogInformation("Listener loop exiting");
    }

    private static string GetSafeFileName(HttpListenerRequest req)
    {
        var fileName = req.Headers["X-FileName"];
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = $"upload_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        }

        // Replace invalid Windows characters with underscores
        var safeName = string.Join("_",
            fileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));

        return safeName;
    }

    static string GetClientFolderName(HttpListenerRequest req)
    {
        // 1) Prefer custom header (client-provided machine name)
        var candidate = req.Headers["X-Client-Host"];

        // 2) If behind a reverse proxy, take the first IP from X-Forwarded-For
        if (string.IsNullOrWhiteSpace(candidate))
        {
            var xff = req.Headers["X-Forwarded-For"];
            if (!string.IsNullOrWhiteSpace(xff))
                candidate = xff.Split(',')[0].Trim();
        }

        //// 3) Fallback to the remote IP from the connection (Kestrel)
        if (string.IsNullOrWhiteSpace(candidate))
            candidate = req.RemoteEndPoint.Address.ToString();

        // 4) Normalize IP and map loopback to "localhost"
        if (!string.IsNullOrWhiteSpace(candidate) && IPAddress.TryParse(candidate, out var ip))
        {
            if (ip.IsIPv4MappedToIPv6)
                ip = ip.MapToIPv4();

            if (IPAddress.IsLoopback(ip))
                candidate = "localhost"; // ::1 or 127.0.0.1
            else
                candidate = ip.ToString(); // normalized textual representation
        }

        // 5) Default
        if (string.IsNullOrWhiteSpace(candidate))
            candidate = "__UNKNOWN_HOST";

        return SanitizeForDirectory(candidate);
    }

    static string SanitizeForDirectory(string name)
    {
        // Replace characters invalid for file/directory names with underscores
        var invalid = Path.GetInvalidFileNameChars();
        var sb = new StringBuilder(name.Length);

        foreach (var ch in name)
            sb.Append(invalid.Contains(ch) || ch is ':' or '/' or '\\' ? '_' : ch);

        return sb.ToString();
    }

    private async Task HandleContextAsync(HttpListenerContext context, CancellationToken ct)
    {
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);

        try
        {
            var req = context.Request;
            var res = context.Response;

            _logger.LogInformation("Incoming request {HttpMethod} {RawUrl} from {Remote}",
                req.HttpMethod, req.RawUrl, req.RemoteEndPoint);

            if (!string.Equals(req.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
            {
                res.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                res.Close();
                return;
            }

            var folder = GetClientFolderName(req);
            var safeName = GetSafeFileName(req);
            var targetDir = Path.Combine(_config.UploadsDirectory, folder);

            Directory.CreateDirectory(targetDir);

            var filePath = Path.Combine(targetDir, safeName);

            _logger.LogInformation("Saving upload to {FilePath}", filePath);

            using (var fs = File.Create(filePath))
            {
                await req.InputStream.CopyToAsync(fs, 81920, ct);
            }

            res.StatusCode = (int)HttpStatusCode.OK;
            res.Close();

            _logger.LogInformation("Upload done: {FilePath}", filePath);
        }
        catch (Exception ex) when (ex is HttpListenerException or ObjectDisposedException)
        {
            _logger.LogDebug(ex, "Request aborted due to shutdown");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling request");

            try
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Close();
            }
            catch { /* ignore */ }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping service…");

        // Stop Accept() and block new connections
        if (_listener.IsListening)
            _listener.Stop();

        // Wait for in-flight requests to complete
        var toWait = _inFlight.Where(t => !t.IsCompleted).ToArray();
        if (toWait.Length > 0)
        {
            _logger.LogInformation("Waiting for {Count} in-flight requests…", toWait.Length);
            await Task.WhenAll(toWait);
        }

        _listener.Close();
        await base.StopAsync(cancellationToken);

        _logger.LogInformation("Service stopped");
    }

    public new void Dispose()
    {
        if (_listener.IsListening)
            _listener.Stop();
        _listener.Close();
        base.Dispose();
    }
}
