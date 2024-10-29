using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoincStatistic.Database.BoincProjectStats;

public interface IBoincProjectStatsRepo
{
    public Task<BoincProjectStatsModel> CreateModel(string name, string category, string totalCredit);

    Task<int> CountAsync();
    
    Task<List<BoincProjectStatsModel>> GetPaginatedAsync(int pageNumber, int pageSize);
}