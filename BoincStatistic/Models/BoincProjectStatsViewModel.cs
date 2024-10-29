using BoincStatistic.Database.BoincProjectStats;

namespace BoincStatistic.Models;

public class BoincProjectStatsViewModel
{
    public List<BoincProjectStatsModel> ProjectStats { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }

    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
}