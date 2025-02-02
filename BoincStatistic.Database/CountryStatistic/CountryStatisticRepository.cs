using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BoincStatistic.Database.CountryStatistic;

public class CountryStatisticRepository : AbstractRepository<CountryStatisticModel>, ICountryStatisticRepository
{
    public CountryStatisticRepository(PostgreSqlContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory)
    {
    }


    public async Task<CountryStatisticModel> CreateModel(
        int projectId,
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
        var model = CountryStatisticModel.CreateModel(projectId, rank, countryName, totalCredit, creditDay, creditWeek, creditMonth, creditAverage, creditUser);

        var result = await CreateModelAsync(model);
        if (result == null)
        {
            throw new Exception("Boinc stats model is not created");
        }

        return result;
    }


    public async Task<List<CountryStatisticModel>> ListAllAsync()
    {
        return await DbModel.OrderBy(x => x.Id).ToListAsync();
    }
    
    
    public async Task<int> CountAsync()
    {
        return await DbModel.CountAsync();
    }

    
    public async Task<List<CountryStatisticModel>> GetThreeCountryAsync()
    {
        var targetCountries = new[] { "Ukraine", "Russian Federation" };

        return await DbModel
            .Where(x => x.Rank == "1" || targetCountries.Contains(x.CountryName))
            .OrderBy(x => x.Id)
            .Take(3)
            .ToListAsync();
    }
    


    
    
    public async Task<List<CountryStatisticModel>> GetPaginatedAsync(int pageNumber, int pageSize)
    {
        return await DbModel
            .OrderBy(b => b.Id)
            .Where(x => x.Rank == "1" || x.CountryName == "Ukraine" || x.CountryName == "Russian Federation")
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    
    public async Task<List<CountryStatisticModel>> ListAllCreditDayData(ImmutableArray<int> projectIds)
    {
        return await DbModel
            .Where(x => projectIds.Contains(x.ProjectId))
            .ToListAsync();
    }    
    
    public async Task<List<CountryStatisticModel>> ListAllCreditDayData(int projectId, ImmutableArray<string> countryNames)
    {
        return await DbModel
            .Where(x => x.ProjectId == projectId)
            .Where(x => countryNames.Contains(x.CountryName))
            .ToListAsync();
    }
}