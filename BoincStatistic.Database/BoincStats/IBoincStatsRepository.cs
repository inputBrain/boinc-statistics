using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoincStatistic.Database.BoincStats;

public interface IBoincStatsRepository
{
    public Task<BoincStatsModel> CreateModel(
        string rank,
        string countryName,
        string totalCredit,
        string creditDay,
        string creditWeek,
        string creditMonth,
        string creditAverage,
        string creditUser
    );
    public Task<List<BoincStatsModel>> ListAllAsync();
}