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
    private static readonly TimeSpan BanDuration = TimeSpan.FromDays(1);

    private readonly ILogger _logger = logger;
    private readonly RemoteUploadConfiguration _config = options.Value;

    // Dictionary to store request counts per IP address: IP -> (Count, WindowStart)
    private readonly ConcurrentDictionary<string, (int Count, DateTime WindowStart)> _ipRateLimits = new();

    // Stores banned IPs: IP -> BanExpirationTime
    private readonly ConcurrentDictionary<string, DateTime> _bannedIps = new();

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

        // Check if IP is currently banned

        if (IsBanned(clientIp))
        {
            _logger.LogWarning("Rejected request from banned IP: {ClientIp}", clientIp);

            response.StatusCode = (int)HttpStatusCode.Forbidden;
            response.StatusDescription = "IP Banned";
            response.Close();

            return false;
        }

        // Rate Limiting Check

        if (IsRateLimited(clientIp))
        {
            _logger.LogWarning("Rate limit exceeded for IP: {ClientIp}", clientIp);

            response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            response.Close();

            _bannedIps.TryAdd(clientIp, DateTime.UtcNow.Add(BanDuration));

            return false;
        }

        // HTTP Method Check

        if (request.HttpMethod != "POST")
        {
            _logger.LogWarning("Invalid HTTP method: {Method} from {ClientIp}", request.HttpMethod, clientIp);

            response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
            response.Close();

            return false;
        }

        // Authorization (Secret Token) Check

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

    /// <summary>
    /// Checks if the IP is in the ban list and if the ban is still active.
    /// automatically removes expired bans.
    /// </summary>
    private bool IsBanned(string ip)
    {
        if (_bannedIps.TryGetValue(ip, out var banExpiration))
        {
            if (DateTime.UtcNow < banExpiration)
            {
                return true; // Still banned
            }

            // Ban expired, remove from list
            _bannedIps.TryRemove(ip, out _);
        }

        return false;
    }
}