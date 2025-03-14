﻿namespace BoincStatistic.Models;

public class TotalScoreViewModel
{
    
    public string TotalCreditUA { get; set; }
    public string TotalCreditRU { get; set; }

    public double TotalUaWeight { get; set; }
    public double TotalRuWeight { get; set; }
    
    public double TotalCreditDifference { get; set; }
    public string TotalAvarageUA { get; set; }
    public string TotalAvarageRU { get; set; }
    public double TotalTaskHours { get; set; }
    public double TotalYearsDifference { get; set; }
    public double TotalMWtPerHourCpu { get; set; }
    public double TotalDevicesToOvercome { get; set; }

    public string ProjectType { get; set; } 

    public string TotalDaysToWin { get; set; }
    
}