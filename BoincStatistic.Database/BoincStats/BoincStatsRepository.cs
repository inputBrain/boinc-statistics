using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BoincStatistic.Database.BoincStats;

public class BoincStatsRepository : AbstractRepository<BoincStatsModel>, IBoincStatsRepository
{
    public BoincStatsRepository(PostgreSqlContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory)
    {
    }


    public async Task<BoincStatsModel> CreateModel(
        string rank,
        string countryName,
        string totalCredit,
        string creditDay,
        string creditWeek,
        string creditMonth,
        string creditAverage,
        string creditUser
    )
    {
        var model = BoincStatsModel.CreateModel(rank, countryName, totalCredit, creditDay, creditWeek, creditMonth, creditAverage, creditUser);

        var result = await CreateModelAsync(model);
        if (result == null)
        {
            throw new Exception("Boinc stats model is not created");
        }

        return result;
    }


    public async Task<List<BoincStatsModel>> ListAllAsync()
    {
        return await DbModel.OrderBy(x => x.Rank).ToListAsync();
    }
    
    
    public async Task<int> CountAsync()
    {
        return await DbModel.CountAsync();
    }

    
    public async Task<List<BoincStatsModel>> GetPaginatedAsync(int pageNumber, int pageSize)
    {
        return await DbModel
            .OrderBy(b => b.Id)
            .Where(x => x.Rank == "1" || x.CountryName == "Ukraine" || x.CountryName == "Russian Federation")
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}