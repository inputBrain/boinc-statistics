using BoincStatistic.Database.BoincProjectStats;
using BoincStatistic.Database.BoincStats;

namespace BoincStatistic.Database;

public interface IDatabaseFacade
{
    IBoincStatsRepository BoincStatsRepository { get; }
    IBoincProjectStatsRepo BoincProjectStatsRepo { get; }
}