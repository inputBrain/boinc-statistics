using System.Threading.Tasks;

namespace BoincStatistic.Database.BoincProjectStats;

public interface IBoincProjectStatsRepo
{
    public Task<BoincProjectStatsModel> CreateModel(string name, string category, string totalCredit);

}