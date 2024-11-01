using System.Collections.Generic;
using System.Threading.Tasks;
using BoincStatistic.Database.BoincStats;

namespace BoincStatistic.Database.BoincProjectStats;

public interface IBoincProjectStatsRepo
{
    
    public Task<BoincProjectStatsModel> CreateModel(string name, string category, string totalCredit);
    
    public Task UpdateModel(BoincProjectStatsModel model, string name, string category, string totalCredit);

    public Task UpdateDetailedStatistics(BoincProjectStatsModel model, BoincStatsModel apiModel);
    
    public Task<BoincProjectStatsModel> GetOneByName(string projectName);

    Task<int> CountAsync();
    
    Task<List<BoincProjectStatsModel>> GetPaginatedAsync(int pageNumber, int pageSize);

    Task<List<BoincProjectStatsModel>> ListAll();
}