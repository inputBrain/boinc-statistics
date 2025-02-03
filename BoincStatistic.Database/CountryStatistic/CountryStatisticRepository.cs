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


    public async Task<bool> CreateBulk(ImmutableArray<CountryStatisticModel> models)
    {
        var result = await CreateBulkModelsAsync(models);
        if (result == null)
        {
            throw new Exception("BitFlyer currency bids collection is not created");
        }

        return true;
    }
    
    
    public async Task<bool> UpdateBulk(ImmutableArray<CountryStatisticModel> models)
    {
        var result = await UpdateBulkModelsAsync(models);
        if (result == null)
        {
            throw new Exception("BitFlyer currency bids collection is not updated");
        }

        return true;
    }


    public async Task<List<CountryStatisticModel>> ListAllAsync()
    {
        return await DbModel.OrderBy(x => x.Id).ToListAsync();
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
    
}