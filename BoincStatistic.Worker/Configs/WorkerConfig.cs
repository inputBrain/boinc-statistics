namespace BoincStatistic.Worker.Configs;

public class WorkerConfig
{
    public bool IsDeveloperMode { get; set; }
    public int SkipFirstN { get; set; } = 0;

    public FlareSolverrConfig FlareSolverr { get; set; } = new();
    public TorConfig Tor { get; set; } = new();
    public RetryConfig Retry { get; set; } = new();
    public ParallelismConfig Parallelism { get; set; } = new();
    public ScrapingConfig Scraping { get; set; } = new();
    public DelayConfig Delay { get; set; } = new();
}

public class FlareSolverrConfig
{
    public string Url { get; set; } = "http://localhost:8191/v1";
    public int MaxTimeoutMs { get; set; } = 90_000;
}

public class TorConfig
{
    public string ControlHost { get; set; } = "localhost";
    public int ControlPort { get; set; } = 9051;
    public string ControlPassword { get; set; } = "";

    public string SocksHostForFlareSolverr { get; set; } = "tor";
    public int SocksPort { get; set; } = 9050;

    public int WaitAfterNewnymSec { get; set; } = 10;
}

public class RetryConfig
{
    public int MaxAttempts { get; set; } = 5;
}

public class ParallelismConfig
{
    public int Projects { get; set; } = 4;
    public int Pages { get; set; } = 2;
}

public class ScrapingConfig
{
    public int MaxPages { get; set; } = 2;
    public int PageSize { get; set; } = 100;
}

public class DelayConfig
{
    public int BetweenProjectsMinMs { get; set; } = 30_000;
    public int BetweenProjectsMaxMs { get; set; } = 60_000;
    public int DevModeMs { get; set; } = 1_000;
}
