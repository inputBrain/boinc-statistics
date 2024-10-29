using System;
using System.Threading.Tasks;
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
}