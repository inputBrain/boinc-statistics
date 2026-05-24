using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using BoincStatistic.Worker.Configs;
using BoincStatistic.Worker.Tor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BoincStatistic.Worker.Scraping;

public class FlareSolverrFetcher : IResilientFetcher
{
    private static readonly string[] BlockMarkers =
    {
        "Unusual behavior detected",
        "Just a moment...",
        "Checking your browser",
        "cf-error-details",
        "Attention Required! | Cloudflare",
        "Access denied | "
    };

    private readonly HttpClient _httpClient;
    private readonly ITorControlClient _torControl;
    private readonly ILogger<FlareSolverrFetcher> _logger;
    private readonly WorkerConfig _config;


    public FlareSolverrFetcher(
        HttpClient httpClient,
        ITorControlClient torControl,
        IOptions<WorkerConfig> options,
        ILogger<FlareSolverrFetcher> logger)
    {
        _httpClient = httpClient;
        _torControl = torControl;
        _logger = logger;
        _config = options.Value;
    }


    public async Task<string> FetchAsync(string targetUrl, string logContext, CancellationToken cancellationToken)
    {
        var maxAttempts = _config.Retry.MaxAttempts;
        var lastReason = "unknown";

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                var html = await _requestOnceAsync(targetUrl, cancellationToken);

                if (string.IsNullOrWhiteSpace(html))
                {
                    lastReason = "empty body";
                }
                else
                {
                    var marker = _findBlockMarker(html);
                    if (marker == null)
                    {
                        if (attempt > 1)
                        {
                            _logger.LogInformation("{Ctx} attempt {Attempt}/{Max} - OK after rotation", logContext, attempt, maxAttempts);
                        }
                        return html;
                    }
                    lastReason = $"blocked by marker: \"{marker}\"";
                }
            }
            catch (Exception ex)
            {
                lastReason = $"exception: {ex.GetType().Name}: {ex.Message}";
                _logger.LogWarning(ex, "{Ctx} attempt {Attempt}/{Max} threw on {Url}", logContext, attempt, maxAttempts, targetUrl);
            }

            _logger.LogWarning("{Ctx} attempt {Attempt}/{Max} - {Reason}. Rotating Tor identity...",
                logContext, attempt, maxAttempts, lastReason);

            if (attempt == maxAttempts)
            {
                break;
            }

            await _torControl.RotateIdentityAsync(cancellationToken);
        }

        throw new BlockedAfterAllRetriesException(targetUrl, maxAttempts, lastReason);
    }


    private async Task<string> _requestOnceAsync(string targetUrl, CancellationToken cancellationToken)
    {
        var payload = new FlareSolverrRequest
        {
            Cmd = "request.get",
            Url = targetUrl,
            MaxTimeout = _config.FlareSolverr.MaxTimeoutMs,
            Proxy = new FlareSolverrProxy
            {
                Url = $"socks5://{_config.Tor.SocksHostForFlareSolverr}:{_config.Tor.SocksPort}"
            }
        };

        using var response = await _httpClient.PostAsJsonAsync(_config.FlareSolverr.Url, payload, cancellationToken);

        FlareSolverrResponse? parsed;
        try
        {
            parsed = await response.Content.ReadFromJsonAsync<FlareSolverrResponse>(cancellationToken: cancellationToken);
        }
        catch
        {
            parsed = null;
        }

        if (!response.IsSuccessStatusCode || parsed?.Status == "error")
        {
            var reason = parsed?.Message ?? $"HTTP {(int)response.StatusCode}";
            throw new FetchFailedException(reason);
        }

        return parsed?.Solution?.Response ?? string.Empty;
    }


    private static string? _findBlockMarker(string html)
    {
        foreach (var marker in BlockMarkers)
        {
            if (html.Contains(marker, StringComparison.OrdinalIgnoreCase))
            {
                return marker;
            }
        }
        return null;
    }


    private class FlareSolverrRequest
    {
        [JsonPropertyName("cmd")] public string Cmd { get; set; } = "";
        [JsonPropertyName("url")] public string Url { get; set; } = "";
        [JsonPropertyName("maxTimeout")] public int MaxTimeout { get; set; }
        [JsonPropertyName("proxy")] public FlareSolverrProxy? Proxy { get; set; }
    }

    private class FlareSolverrProxy
    {
        [JsonPropertyName("url")] public string Url { get; set; } = "";
    }

    private class FlareSolverrResponse
    {
        [JsonPropertyName("solution")] public FlareSolverrSolution? Solution { get; set; }
        [JsonPropertyName("status")] public string? Status { get; set; }
        [JsonPropertyName("message")] public string? Message { get; set; }
    }

    private class FlareSolverrSolution
    {
        [JsonPropertyName("response")] public string? Response { get; set; }
        [JsonPropertyName("status")] public int Status { get; set; }
    }
}
