using BoincStatistic.Database.BoincStats;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BoincStatistic.Database;

public class PostgreSqlContext : DbContext
{
    public readonly IDatabaseFacade Db;

    public DbSet<BoincStatsModel> BoincStats { get; set; }


    public PostgreSqlContext(DbContextOptions<PostgreSqlContext> options, ILoggerFactory loggerFactory) : base(options)
    {
        Db = new DatabaseFacade(this, loggerFactory);
    }
}