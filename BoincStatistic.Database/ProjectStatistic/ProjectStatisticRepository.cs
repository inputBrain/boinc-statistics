using System;
using System.Collections.Generic;
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

    
    public async Task UpdateModel(ProjectStatisticModel model, string name, string category, string totalCredit)
    {
        model.UpdateTotalStatsModel(model, name, category, totalCredit);
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
            .OrderBy(p => p.ProjectName == "Total without ASIC" ? 1 : 0)
            .ToList();
    }
}