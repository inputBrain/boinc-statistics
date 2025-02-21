using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using BoincStatistic.Database.CountryStatistic;

namespace BoincStatistic.Database.ProjectStatistic;

public interface IProjectStatisticRepository
{
    
    public Task<ProjectStatisticModel> CreateModel(string name, string category, string totalCredit);
    
    public Task UpdateModel(ProjectStatisticModel model, string totalCredit);
    
    Task<bool> UpdateBulk(ImmutableArray<ProjectStatisticModel> models);

    Task UpdateUpdateAt(ProjectStatisticModel model, DateTimeOffset updatedAt);
    
    public Task UpdateDetailedStatistics(ProjectStatisticModel model, CountryStatisticModel apiModel);
    
    public Task<ProjectStatisticModel> GetOneByName(string projectName);

    Task<int> CountAsync();

    Task SetProjectStatus(ProjectStatisticModel model, ScrappingStatus scrappingStatus);

    Task SetToAllProjectsInWaitingStatus();
    
    Task<List<ProjectStatisticModel>> GetPaginatedAsync(int pageNumber, int pageSize);

    Task<List<ProjectStatisticModel>> ListAll();
    
    Task<ImmutableArray<ProjectStatisticModel>> List();
}