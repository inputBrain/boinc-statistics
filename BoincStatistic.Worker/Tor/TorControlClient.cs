using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BoincStatistic.Worker.Configs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BoincStatistic.Worker.Tor;

public class TorControlClient : ITorControlClient
{
    private readonly TorConfig _config;
    private readonly ILogger<TorControlClient> _logger;
    private readonly SemaphoreSlim _rotationLock = new(1, 1);
    private DateTime _lastRotationAt = DateTime.MinValue;


    public TorControlClient(IOptions<WorkerConfig> options, ILogger<TorControlClient> logger)
    {
        _config = options.Value.Tor;
        _logger = logger;
    }


    public async Task<bool> RotateIdentityAsync(CancellationToken cancellationToken)
    {
        var waitWindow = TimeSpan.FromSeconds(_config.WaitAfterNewnymSec);
        DateTime lastRotation;

        await _rotationLock.WaitAsync(cancellationToken);
        try
        {
            var since = DateTime.UtcNow - _lastRotationAt;
            if (since < waitWindow)
            {
                _logger.LogInformation("Tor rotation debounced (last NEWNYM was {Sec}s ago, window {Window}s)",
                    (int)since.TotalSeconds, _config.WaitAfterNewnymSec);
                lastRotation = _lastRotationAt;
            }
            else
            {
                var ok = await _sendNewnymAsync(cancellationToken);
                if (!ok)
                {
                    return false;
                }
                _lastRotationAt = DateTime.UtcNow;
                lastRotation = _lastRotationAt;
                _logger.LogInformation("Tor identity rotated (NEWNYM)");
            }
        }
        finally
        {
            _rotationLock.Release();
        }

        var remaining = waitWindow - (DateTime.UtcNow - lastRotation);
        if (remaining > TimeSpan.Zero)
        {
            await Task.Delay(remaining, cancellationToken);
        }
        return true;
    }


    private async Task<bool> _sendNewnymAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(_config.ControlHost, _config.ControlPort, cancellationToken);

            await using var stream = tcpClient.GetStream();
            using var reader = new StreamReader(stream, Encoding.ASCII);
            await using var writer = new StreamWriter(stream, Encoding.ASCII) { NewLine = "\r\n", AutoFlush = true };

            await writer.WriteLineAsync($"AUTHENTICATE \"{_config.ControlPassword}\"");
            var authResponse = await reader.ReadLineAsync(cancellationToken);
            if (authResponse == null || !authResponse.StartsWith("250"))
            {
                _logger.LogError("Tor AUTHENTICATE failed: {Response}", authResponse);
                return false;
            }

            await writer.WriteLineAsync("SIGNAL NEWNYM");
            var signalResponse = await reader.ReadLineAsync(cancellationToken);
            if (signalResponse == null || !signalResponse.StartsWith("250"))
            {
                _logger.LogError("Tor SIGNAL NEWNYM failed: {Response}", signalResponse);
                return false;
            }

            await writer.WriteLineAsync("QUIT");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tor NEWNYM rotation crashed on {Host}:{Port}", _config.ControlHost, _config.ControlPort);
            return false;
        }
    }
}
