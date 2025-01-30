using BoincStatistic.Database.CountryStatistic;
using BoincStatistic.Database.ProjectStatistic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BoincStatistic.Database;

public class PostgreSqlContext : DbContext
{
    public readonly IDatabaseFacade Db;
    
    public DbSet<ProjectStatisticModel> ProjectStats { get; set; }
    public DbSet<CountryStatisticModel> DetailedProjectStats { get; set; }


    public PostgreSqlContext(DbContextOptions<PostgreSqlContext> options, ILoggerFactory loggerFactory) : base(options)
    {
        Db = new DatabaseFacade(this, loggerFactory);
    }
}