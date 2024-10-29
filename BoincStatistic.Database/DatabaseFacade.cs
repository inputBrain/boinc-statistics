using BoincStatistic.Database.BoincProjectStats;
using BoincStatistic.Database.BoincStats;
using Microsoft.Extensions.Logging;

namespace BoincStatistic.Database;

public class DatabaseFacade : IDatabaseFacade
{
    public IBoincStatsRepository BoincStatsRepository { get; set; }

    public IBoincProjectStatsRepo BoincProjectStatsRepo { get; set; }


    public DatabaseFacade(PostgreSqlContext context, ILoggerFactory loggerFactory)
    {
        BoincStatsRepository = new BoincStatsRepository(context, loggerFactory);
        BoincProjectStatsRepo = new BoincProjectStatsRepo(context, loggerFactory);
    }

}