using System;
using BoincStatistic.Database;
using BoincStatistic.Worker;
using BoincStatistic.Worker.Configs;
using BoincStatistic.Worker.Scraping;
using BoincStatistic.Worker.Tor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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

            services.Configure<WorkerConfig>(hostContext.Configuration.GetSection("WorkerConfig"));

            services.AddDbContext<PostgreSqlContext>(opt => opt.UseNpgsql
            (
                hostContext.Configuration.GetConnectionString("PostgreSqlConnection")
            ));

            services.AddSingleton<ITorControlClient, TorControlClient>();
            services.AddHttpClient<IResilientFetcher, FlareSolverrFetcher>((sp, client) =>
            {
                var cfg = sp.GetRequiredService<IOptions<WorkerConfig>>().Value;
                client.Timeout = TimeSpan.FromMilliseconds(cfg.FlareSolverr.HttpTimeoutMs);
            });

            services.AddHostedService<BoincStatsService>();
        })
    .Build();

host.Run();
