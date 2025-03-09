using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using BoincStatistic.Database.CountryStatistic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BoincStatistic.Database.ProjectStatistic;

public class ProjectStatisticRepository : AbstractRepository<ProjectStatisticModel>, IProjectStatisticRepository
{
    public ProjectStatisticRepository(PostgreSqlContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory)
    {
    }


    public async Task<ProjectStatisticModel> GetOneByName(string projectName)
    {
        var model = await DbModel
            .Include(x => x.CountryStatistics)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.ProjectName.ToLower() == projectName.ToLower());

        if (model == null)
        {
            Logger.LogError("Project {0} is not found in DB.", projectName);
            return null;
        }

        return model;
    }


    public async Task<ProjectStatisticModel> CreateModel(string name, string category, string totalCredit)
    {
        var model = ProjectStatisticModel.CreateModel(name, category, totalCredit);

        var result = await CreateModelAsync(model);
        if (result == null)
        {
            throw new Exception("Project stats model is not created");
        }

        return result;
    }

    
    public async Task UpdateModel(ProjectStatisticModel model, string totalCredit)
    {
        model.UpdateTotalStatsModel(model, totalCredit);
        await UpdateModelAsync(model);
    }


    public async Task<bool> UpdateBulk(ImmutableArray<ProjectStatisticModel> models)
    {
        var result = await UpdateBulkModelsAsync(models);
        if (result == null)
        {
            throw new Exception("BitFlyer currency bids collection is not updated");
        }

        return true;
    }


    public async Task UpdateUpdateAt(ProjectStatisticModel model, DateTimeOffset updatedAt)
    {
        model.UpdatedAt = updatedAt;
        await UpdateModelAsync(model);
    }


    public async Task UpdateDetailedStatistics(ProjectStatisticModel model, CountryStatisticModel apiModel)
    {
        model.UpdateDetailedStatistics(model, apiModel);
        await UpdateModelAsync(model);
    }

    
    public async Task<int> CountAsync()
    {
        return await DbModel.CountAsync();
    }


    public async Task SetProjectStatus(ProjectStatisticModel model, ScrappingStatus scrappingStatus)
    {
        model.Status = scrappingStatus;
        await UpdateModelAsync(model);
    }


    public async Task SetToAllProjectsInWaitingStatus()
    {
        await DbModel.ExecuteUpdateAsync(s => s.SetProperty(p => p.Status, ScrappingStatus.InWaiting));
    }


    public async Task<List<ProjectStatisticModel>> GetPaginatedAsync(int pageNumber, int pageSize)
    {
        return await DbModel
            .OrderBy(b => b.Id)
            .Include(x => x.CountryStatistics)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsSplitQuery()
            .ToListAsync();
    }


    public async Task<List<ProjectStatisticModel>> ListAll()
    {
        var projectList = await DbModel
            .Include(x => x.CountryStatistics)
            .AsSplitQuery()
            .ToListAsync();

        return projectList
            .OrderBy(p => p.ProjectName == "Total without ASIC")
            .ThenByDescending(x => x.Type == ProjectType.Core)
            .ThenBy(p => p.ProjectName)
            .ToList();
    }
    
    
    public async Task<ImmutableArray<ProjectStatisticModel>> List()
    {
        var collection =await DbModel
            .OrderBy(b => b.Id)
            .Include(x => x.CountryStatistics)
            .AsSplitQuery()
            .ToListAsync();

        return [..collection];
    }
}