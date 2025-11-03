using BoincStatistic.Database.ProjectStatistic;

namespace BoincStatistic.Models;

public class ProjectsSimpleViewModel
{
    public string ProjectName { get; set; }
    public string ProjectStatsUrl { get; set; }
    
    public string TotalCredit { get; set; }
    
    public string Category { get; set; }
    
    public ScrappingStatus Status { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
    
    public bool IsCreditDayZero { get; set; }

    public int Divider { get; set; }
    
    public string ProjectType { get; set; } 


    public DateTimeOffset GetKyivTime()
    {
        var userTimeZone = GetKyivTimeZone();
        return TimeZoneInfo.ConvertTime(UpdatedAt, userTimeZone);
    }

    private static TimeZoneInfo GetKyivTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Europe/Kyiv");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev");
        }
    }
}