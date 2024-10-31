using BoincStatistic.Database.BoincProjectStats;
using BoincStatistic.Database.BoincStats;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BoincStatistic.Database;

public class PostgreSqlContext : DbContext
{
    public readonly IDatabaseFacade Db;
    
    public DbSet<BoincProjectStatsModel> ProjectStats { get; set; }
    public DbSet<BoincStatsModel> DetailedProjectStats { get; set; }


    public PostgreSqlContext(DbContextOptions<PostgreSqlContext> options, ILoggerFactory loggerFactory) : base(options)
    {
        Db = new DatabaseFacade(this, loggerFactory);
    }
}