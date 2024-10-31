namespace BoincStatistic.Models;

public class ProjectWeightViewModel
{
    public string ProjectName { get; set; }
    public double TotalCredit { get; set; }
    public double UaWeight { get; set; }
    public double RuWeight { get; set; }
    public double CreditDifference { get; set; }
    public double TaskHours { get; set; }
    public double YearsDifference { get; set; }
    public double MWtPerHourCpu { get; set; }
    public double DaysToWin { get; set; }
    
    public string CreditUA { get; set; }
    public string CreditRU { get; set; }
    public string AvarageRU { get; set; }
    public string AvarageUA { get; set; }
}