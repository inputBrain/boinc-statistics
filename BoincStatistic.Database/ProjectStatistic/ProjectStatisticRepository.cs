using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BoincStatistic.Database.ProjectStatistic;

public class ProjectStatisticRepository : AbstractRepository<ProjectStatisticModel>, IProjectStatisticRepository
{
    public ProjectStatisticRepository(PostgreSqlContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory)
    {
    }


    public async Task UpdateModel(ProjectStatisticModel model, string totalCredit, bool isCreditDayZero)
    {
        model.UpdateTotalStatsModel(model, totalCredit, isCreditDayZero);
        await UpdateModelAsync(model);
    }


    public async Task UpdateUpdateAt(ProjectStatisticModel model, DateTimeOffset updatedAt)
    {
        model.UpdatedAt = updatedAt;
        await UpdateModelAsync(model);
    }


    public async Task SetProjectStatus(ProjectStatisticModel model, ScrappingStatus scrappingStatus)
    {
        model.Status = scrappingStatus;
        await UpdateModelAsync(model);
    }


    public async Task SetToAllProjectsInWaitingStatus()
    {
        await DbModel.Where(x => x.IsScrappingActive == true).ExecuteUpdateAsync(s => s.SetProperty(p => p.Status, ScrappingStatus.InWaiting));
    }


    public async Task<List<ProjectStatisticModel>> ListAll()
    {
        var projectList = await DbModel
            .Where(x => x.IsScrappingActive == true)
            .Include(x => x.CountryStatistics)
            .AsSplitQuery()
            .ToListAsync();

        return projectList.OrderBy(p => p.ProjectName == "Total without ASIC")
            .ThenByDescending(x => x.Type == ProjectType.Core)
            .ThenBy(p => p.ProjectName)
            .ToList();
    }
}