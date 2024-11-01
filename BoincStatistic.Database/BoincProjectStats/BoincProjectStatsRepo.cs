using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoincStatistic.Database.BoincStats;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BoincStatistic.Database.BoincProjectStats;

public class BoincProjectStatsRepo : AbstractRepository<BoincProjectStatsModel>, IBoincProjectStatsRepo
{
    public BoincProjectStatsRepo(PostgreSqlContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory)
    {
    }


    public async Task<BoincProjectStatsModel> GetOneByName(string projectName)
    {
        var model = await DbModel
            .Include(x => x.DetailedStatistics)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.ProjectName.ToLower() == projectName.ToLower());

        if (model == null)
        {
            Logger.LogError("Project {0} is not found in DB.", projectName);
            return null;
        }

        return model;
    }


    public async Task<BoincProjectStatsModel> CreateModel(string name, string category, string totalCredit)
    {
        var model = BoincProjectStatsModel.CreateModel(name, category, totalCredit);

        var result = await CreateModelAsync(model);
        if (result == null)
        {
            throw new Exception("Project stats model is not created");
        }

        return result;
    }

    
    public async Task UpdateModel(BoincProjectStatsModel model, string name, string category, string totalCredit)
    {
        model.UpdateTotalStatsModel(model, name, category, totalCredit);
        await UpdateModelAsync(model);
    }
    
    
    public async Task UpdateDetailedStatistics(BoincProjectStatsModel model, BoincStatsModel apiModel)
    {
        model.UpdateDetailedStatistics(model, apiModel);
        await UpdateModelAsync(model);
    }

    
    public async Task<int> CountAsync()
    {
        return await DbModel.CountAsync();
    }


    public async Task<List<BoincProjectStatsModel>> GetPaginatedAsync(int pageNumber, int pageSize)
    {
        return await DbModel
            .OrderBy(b => b.Id)
            .Include(x => x.DetailedStatistics)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsSplitQuery()
            .ToListAsync();
    }


    public async Task<List<BoincProjectStatsModel>> ListAll()
    {
        var projectList = await DbModel
            .Include(x => x.DetailedStatistics)
            .AsSplitQuery()
            .ToListAsync();

        return projectList
            .OrderBy(p => p.ProjectName == "Total without ASIC" ? 1 : 0)
            .ToList();
    }
}