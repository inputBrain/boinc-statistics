using BoincStatistic.Database.CountryStatistic;
using BoincStatistic.Database.ProjectStatistic;
using Microsoft.Extensions.Logging;

namespace BoincStatistic.Database;

public class DatabaseFacade : IDatabaseFacade
{
    public ICountryStatisticRepository CountryStatisticRepository { get; set; }

    public IProjectStatisticRepository ProjectStatisticRepository { get; set; }



    public DatabaseFacade(PostgreSqlContext context, ILoggerFactory loggerFactory)
    {
        CountryStatisticRepository = new CountryStatisticRepository(context, loggerFactory);
        ProjectStatisticRepository = new ProjectStatisticRepository(context, loggerFactory);
    }

}