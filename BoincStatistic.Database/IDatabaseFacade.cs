using BoincStatistic.Database.CountryStatistic;
using BoincStatistic.Database.ProjectStatistic;

namespace BoincStatistic.Database;

public interface IDatabaseFacade
{
    ICountryStatisticRepository CountryStatisticRepository { get; }
    IProjectStatisticRepository ProjectStatisticRepository { get; }
}