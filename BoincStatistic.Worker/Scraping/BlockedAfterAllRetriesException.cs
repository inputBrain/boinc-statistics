using System;

namespace BoincStatistic.Worker.Scraping;

public class BlockedAfterAllRetriesException : Exception
{
    public string Url { get; }
    public int Attempts { get; }
    public string LastReason { get; }

    public BlockedAfterAllRetriesException(string url, int attempts, string lastReason)
        : base($"Blocked after {attempts} attempts on {url}. Last reason: {lastReason}")
    {
        Url = url;
        Attempts = attempts;
        LastReason = lastReason;
    }
}
