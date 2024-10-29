using System;
using BoincStatistic.Database;
using BoincStatistic.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

            services.AddHostedService<BoincStatsService>();
            services.AddHostedService<BoincProjectStatsService>();

        })
    .Build();

host.Run();