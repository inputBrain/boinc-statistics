using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using BoincStatistic.Database.CountryStatistic;

namespace BoincStatistic.Database.ProjectStatistic;

public interface IProjectStatisticRepository
{
    
    public Task<ProjectStatisticModel> CreateModel(string name, string category, string totalCredit);
    
    public Task UpdateModel(ProjectStatisticModel model, string name, string category, string totalCredit);

    public Task UpdateDetailedStatistics(ProjectStatisticModel model, CountryStatisticModel apiModel);
    
    public Task<ProjectStatisticModel> GetOneByName(string projectName);

    Task<int> CountAsync();
    
    Task<List<ProjectStatisticModel>> GetPaginatedAsync(int pageNumber, int pageSize);

    Task<List<ProjectStatisticModel>> ListAll();
    
    Task<ImmutableArray<ProjectStatisticModel>> List();
}