using System;
using BoincStatistic.Database;
using BoincStatistic.Database.BoincProjectStats;
using BoincStatistic.Worker;
using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(
        config =>
        {
            config.AddJsonFile("appsettings.json")
                .Build();
        })
    .ConfigureServices(
        (hostContext, services) =>
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddDbContext<PostgreSqlContext>(opt => opt.UseNpgsql
            (
               hostContext.Configuration.GetConnectionString("PostgreSqlConnection")
            ));
            //
            // services.AddHttpClient<IBaseParser, BaseParser>();
            //
            // services.AddSingleton<IGateioParser, GateioParser>();
            //
            // services.AddHostedService<BoincStatsService>();
            services.AddHostedService<BoincProjectStatsService>();
            //
            // services.AddSingleton<IUseCaseContainer>(
            //     sp => Factory.Create(
            //         sp.GetRequiredService<ILoggerFactory>(),
            //         sp.GetRequiredService<IGateioParser>()
            //     )
            // );
        })
    .Build();

host.Run();