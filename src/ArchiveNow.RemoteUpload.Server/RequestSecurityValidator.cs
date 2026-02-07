using System.Collections.Concurrent;
using System.Net;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ArchiveNow.RemoteUpload.Server;

/// <summary>
/// Handles request validation logic including rate limiting, method checks, and authentication.
/// </summary>
public class RequestSecurityValidator(IOptions<RemoteUploadConfiguration> options, ILogger logger)
{
    private readonly ILogger _logger = logger;
    private readonly RemoteUploadConfiguration _config = options.Value;

    // Dictionary to store request counts per IP address: IP -> (Count, WindowStart)
    private readonly ConcurrentDictionary<string, (int Count, DateTime WindowStart)> _ipRateLimits = new();

    /// <summary>
    /// Validates the incoming request against security rules.
    /// If validation fails, it sets the response status code, closes the response, and returns false.
    /// </summary>
    /// <param name="context">The HTTP listener context.</param>
    /// <returns>True if the request is valid and should be processed; otherwise, false.</returns>
    public bool Validate(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        string clientIp = request.RemoteEndPoint?.Address.ToString() ?? "unknown";

        // 1. Rate Limiting Check

        if (IsRateLimited(clientIp))
        {
            _logger.LogWarning("Rate limit exceeded for IP: {ClientIp}", clientIp);

            response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            response.Close();

            return false;
        }

        // 2. HTTP Method Check

        if (request.HttpMethod != "POST")
        {
            _logger.LogWarning("Invalid HTTP method: {Method} from {ClientIp}", request.HttpMethod, clientIp);

            response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
            response.Close();

            return false;
        }

        // 3. Authorization (Secret Token) Check

        var accessSecret = request.Headers["X-Access-Secret"];
        if (string.IsNullOrWhiteSpace(accessSecret) || accessSecret != _config.AccessSecret)
        {
            _logger.LogWarning("Unauthorized access attempt from {ClientIp}", clientIp);

            response.StatusCode = (int)HttpStatusCode.Unauthorized;
            response.Close();

            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if the IP address has exceeded the request limit.
    /// </summary>
    private bool IsRateLimited(string ip)
    {
        var now = DateTime.UtcNow;
        var limit = _config.MaxRequestsPerMinute;

        var stats = _ipRateLimits.AddOrUpdate(ip,
            (1, now), // New entry
            (key, oldStats) =>
            {
                if ((now - oldStats.WindowStart).TotalMinutes >= 1)
                {
                    return (1, now); // Reset window
                }
                return (oldStats.Count + 1, oldStats.WindowStart); // Increment
            });

        return stats.Count > limit;
    }
}