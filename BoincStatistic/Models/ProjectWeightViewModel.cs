namespace BoincStatistic.Models;

public class ProjectWeightViewModel
{
    public string Country { get; set; }
    public string ProjectName { get; set; }
    public double UaWeight { get; set; }
    public double RuWeight { get; set; }
    public string CreditDifference { get; set; }
    public string CreditUA { get; set; }
    public string CreditRU { get; set; }
    public string AvarageRU { get; set; }
    public string AvarageUA { get; set; }
    public string TaskHours { get; set; }
    public string YearsDifference { get; set; }
    public string MWtPerHourCpu { get; set; }
    public double DevicesToOvercome { get; set; }
    public string DaysToWin { get; set; }
    
    public string ProjectType { get; set; } 
}