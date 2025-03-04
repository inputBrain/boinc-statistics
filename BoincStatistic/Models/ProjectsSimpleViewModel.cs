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
    
    public bool HasMoreThanZeroCreditDay { get; set; }


    public DateTimeOffset GetKyivTime()
    {
        var userTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev");
        return TimeZoneInfo.ConvertTime(UpdatedAt, userTimeZone);
    }
}