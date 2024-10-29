using BoincStatistic.Database.BoincStats;

namespace BoincStatistic.Models;

public class BoincStatsViewModel
{
    public List<BoincStatsModel> BoincStats { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }

    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
}