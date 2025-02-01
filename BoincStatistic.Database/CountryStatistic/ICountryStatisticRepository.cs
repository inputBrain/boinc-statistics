using System.Collections.Generic;
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

    Task<bool> CountCreditDayRows(int projectId);


    public Task<CountryStatisticModel> GetOneCountryStatsByCountryName(string country, int projectId);
    public Task<CountryStatisticModel> GetOneByRank(string rank, int projectId);
    
    public Task<List<CountryStatisticModel>> ListAllAsync();
    
    Task<int> CountAsync();
    
    Task<List<CountryStatisticModel>> GetThreeCountryAsync(int projectId);

    Task<List<CountryStatisticModel>> GetPaginatedAsync(int pageNumber, int pageSize);
}