using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BoincStatistic.Database.BoincProjectStats;

public class BoincProjectStatsRepo : AbstractRepository<BoincProjectStatsModel>, IBoincProjectStatsRepo
{
    public BoincProjectStatsRepo(PostgreSqlContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory)
    {
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
    
    public async Task<BoincProjectStatsModel> GetOneProjectStatsByProjectName(string project)
    {
        var model = await DbModel.FirstOrDefaultAsync(x => x.ProjectName == project);
        if (model == null)
        {
            throw new Exception("Country stats is not found");
        }

        return model;
    }
    

    public async Task<int> CountAsync()
    {
        return await DbModel.CountAsync();
    }


    public async Task<List<BoincProjectStatsModel>> GetPaginatedAsync(int pageNumber, int pageSize)
    {
        return await DbModel.OrderBy(b => b.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }


    public async Task<List<BoincProjectStatsModel>> ListAll()
    {
       return await DbModel.ToListAsync();
    }
}