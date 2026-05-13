namespace BoincStatistic.Worker.Configs;

public class WorkerConfig
{
    public bool IsDeveloperMode { get; set; }
    public string FlareSolverrUrl { get; set; } = "http://localhost:8191/v1";
}