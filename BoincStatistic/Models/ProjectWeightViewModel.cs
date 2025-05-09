﻿namespace BoincStatistic.Models;

public class ProjectWeightViewModel
{
    public string Country { get; set; }
    public string ProjectName { get; set; }
    
    public string ProjectStatsUrl { get; set; }
    public string CountryStatsUrl { get; set; }
    public double UaWeight { get; set; }
    public double RuWeight { get; set; }
    public double CreditDifference { get; set; }
    public string CreditUA { get; set; }
    public string CreditRU { get; set; }
    public string AvarageRU { get; set; }
    public string AvarageUA { get; set; }
    public double TaskHours { get; set; }
    public double YearsDifference { get; set; }
    public double MWtPerHourCpu { get; set; }
    public double DevicesToOvercome { get; set; }
    public string DaysToWin { get; set; }
    
    public string ProjectType { get; set; } 
    
    public bool IsCreditDayZero { get; set; }
    
    public string DaysToWinWithMinus { get; set; } 

}