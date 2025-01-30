using BoincStatistic.Database.ProjectStatistic;

namespace BoincStatistic.Models;

public class BoincProjectStatsViewModel
{
    public List<ProjectStatisticModel> ProjectStats { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }

    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
}