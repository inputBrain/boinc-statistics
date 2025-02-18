using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace BoincStatistic.Database.CountryStatistic;

public interface ICountryStatisticRepository
{
    public Task<CountryStatisticModel> CreateModel(
        int projectId,
        string rank,
        string countryName,
        string totalCredit,
        string creditDay,
        string creditWeek,
        string creditMonth,
        string creditAverage,
        string creditUser
    );
    
    Task<bool> CreateBulk(ImmutableArray<CountryStatisticModel> models);
    Task<bool> UpdateBulk(ImmutableArray<CountryStatisticModel> models);
    
    public Task<List<CountryStatisticModel>> ListAllAsync();

    Task<List<CountryStatisticModel>> ListAllCreditDayData(ImmutableArray<int> projectIds);
    
    
    Task<List<CountryStatisticModel>> GetThreeCountryAsync();

    Task<List<CountryStatisticModel>> GetPaginatedAsync(int pageNumber, int pageSize);
}