using BoincStatistic.Database.ProjectStatistic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoincStatistic.Migrations
{
    /// <inheritdoc />
    public partial class projectsSeed : Migration
    {
        private List<ProjectStatisticModel> ProjectConfigs = new List<ProjectStatisticModel>()
        {
            new ProjectStatisticModel()
            {
                ProjectName = "GPUGRID",
                ProjectCategory = "Biology",
                ProjectStatisticUrl = "https://www.boincstats.com/stats/45/project/detail/",
                CountryStatisticUrl = "https://www.boincstats.com/stats/45/country/list",
                Type = ProjectType.GPU,
                Divider = 100000,
                DefaultDivider = 22500,
                Status = ScrappingStatus.InWaiting,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            },
            new ProjectStatisticModel()
            {
                ProjectName = "NumberFields",
                ProjectCategory = "Mathematics",
                ProjectStatisticUrl = "https://www.boincstats.com/stats/122/project/detail/",
                CountryStatisticUrl = "https://www.boincstats.com/stats/122/country/list",
                Type = ProjectType.GPU,
                Divider = 2000,
                DefaultDivider = 22500,
                Status = ScrappingStatus.InWaiting,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            },
            new ProjectStatisticModel()
            {
                ProjectName = "NFS",
                ProjectCategory = "Mathematics",
                ProjectStatisticUrl = "https://www.boincstats.com/stats/88/project/detail/",
                CountryStatisticUrl = "https://www.boincstats.com/stats/88/country/list",
                Type = ProjectType.Core,
                Divider = 90,
                DefaultDivider = 22500,
                Status = ScrappingStatus.InWaiting,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            },
            new ProjectStatisticModel()
            {
                ProjectName = "Total without ASIC",
                ProjectCategory = "Uncategorized",
                ProjectStatisticUrl = "https://www.boincstats.com/stats/-5/project/detail/",
                CountryStatisticUrl = "https://www.boincstats.com/stats/-5/country/list",
                Type = ProjectType.GPU,
                Divider = 22500,
                DefaultDivider = 22500,
                Status = ScrappingStatus.InWaiting,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            },
            new ProjectStatisticModel()
            {
                ProjectName = "Asteroids",
                ProjectCategory = "Astrophysics",
                ProjectStatisticUrl = "https://www.boincstats.com/stats/134/project/detail/",
                CountryStatisticUrl = "https://www.boincstats.com/stats/134/country/list",
                Type = ProjectType.Core,
                Divider = 45,
                DefaultDivider = 22500,
                Status = ScrappingStatus.InWaiting,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            },
            new ProjectStatisticModel()
            {
                ProjectName = "Climate Prediction",
                ProjectCategory = "Earth Sciences",
                ProjectStatisticUrl = "https://www.boincstats.com/stats/2/project/detail/",
                CountryStatisticUrl = "https://www.boincstats.com/stats/2/country/list",
                Type = ProjectType.Core,
                Divider = 70,
                DefaultDivider = 22500,
                Status = ScrappingStatus.InWaiting,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            },
            new ProjectStatisticModel()
            {
                ProjectName = "LODA",
                ProjectCategory = "Artificial intelligence",
                ProjectStatisticUrl = "https://www.boincstats.com/stats/199/project/detail/",
                CountryStatisticUrl = "https://www.boincstats.com/stats/199/country/list",
                Type = ProjectType.Core,
                Divider = 50,
                DefaultDivider = 22500,
                Status = ScrappingStatus.InWaiting,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            },
            new ProjectStatisticModel()
            {
                ProjectName = "Milkyway",
                ProjectCategory = "Astrophysics",
                ProjectStatisticUrl = "https://www.boincstats.com/stats/61/project/detail/",
                CountryStatisticUrl = "https://www.boincstats.com/stats/61/country/list",
                Type = ProjectType.Core,
                Divider = 50,
                DefaultDivider = 22500,
                Status = ScrappingStatus.InWaiting,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            },
            new ProjectStatisticModel()
            {
                ProjectName = "Rosetta",
                ProjectCategory = "Biology",
                ProjectStatisticUrl = "https://www.boincstats.com/stats/14/project/detail/",
                CountryStatisticUrl = "https://www.boincstats.com/stats/14/country/list",
                Type = ProjectType.Core,
                Divider = 40,
                DefaultDivider = 22500,
                Status = ScrappingStatus.InWaiting,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            },
            new ProjectStatisticModel()
            {
                ProjectName = "World Community Grid",
                ProjectCategory = "Umbrella project",
                ProjectStatisticUrl = "https://www.boincstats.com/stats/15/project/detail/",
                CountryStatisticUrl = "https://www.boincstats.com/stats/15/country/list",
                Type = ProjectType.Core,
                Divider = 50,
                DefaultDivider = 22500,
                Status = ScrappingStatus.InWaiting,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            },
            new ProjectStatisticModel()
            {
                ProjectName = "YAFU",
                ProjectCategory = "Mathematics",
                ProjectStatisticUrl = "https://www.boincstats.com/stats/121/project/detail/",
                CountryStatisticUrl = "https://www.boincstats.com/stats/121/country/list",
                Type = ProjectType.Core,
                Divider = 40,
                DefaultDivider = 22500,
                Status = ScrappingStatus.InWaiting,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            },
            new ProjectStatisticModel()
            {
                ProjectName = "Yoyo",
                ProjectCategory = "Umbrella project",
                ProjectStatisticUrl = "https://www.boincstats.com/stats/52/project/detail/",
                CountryStatisticUrl = "https://www.boincstats.com/stats/52/country/list",
                Type = ProjectType.Core,
                Divider = 40,
                DefaultDivider = 22500,
                Status = ScrappingStatus.InWaiting,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            },
            new ProjectStatisticModel()
            {
                ProjectName = "Amicable Numbers",
                ProjectCategory = "Mathematics",
                ProjectStatisticUrl = "https://www.boincstats.com/stats/172/project/detail/",
                CountryStatisticUrl = "https://www.boincstats.com/stats/172/country/list",
                Type = ProjectType.GPU,
                Divider = 55000,
                DefaultDivider = 22500,
                Status = ScrappingStatus.InWaiting,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            },
            new ProjectStatisticModel()
            {
                ProjectName = "Einstein",
                ProjectCategory = "Astrophysics",
                ProjectStatisticUrl = "https://www.boincstats.com/stats/5/project/detail/",
                CountryStatisticUrl = "https://www.boincstats.com/stats/5/country/list",
                Type = ProjectType.GPU,
                Divider = 20000,
                DefaultDivider = 22500,
                Status = ScrappingStatus.InWaiting,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            },
            new ProjectStatisticModel()
            {
                ProjectName = "Moo! Wrapper",
                ProjectCategory = "Mathematics",
                ProjectStatisticUrl = "https://www.boincstats.com/stats/114/project/detail/",
                CountryStatisticUrl = "https://www.boincstats.com/stats/114/country/list",
                Type = ProjectType.GPU,
                Divider = 45000,
                DefaultDivider = 22500,
                Status = ScrappingStatus.InWaiting,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            },
            new ProjectStatisticModel()
            {
                ProjectName = "PrimeGrid",
                ProjectCategory = "Mathematics",
                ProjectStatisticUrl = "https://www.boincstats.com/stats/11/project/detail/",
                CountryStatisticUrl = "https://www.boincstats.com/stats/11/country/list",
                Type = ProjectType.GPU,
                Divider = 7500,
                DefaultDivider = 22500,
                Status = ScrappingStatus.InWaiting,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            },
            new ProjectStatisticModel()
            {
                ProjectName = "LHC",
                ProjectCategory = "Physics",
                ProjectStatisticUrl = "https://www.boincstats.com/stats/3/project/detail/",
                CountryStatisticUrl = "https://www.boincstats.com/stats/3/country/list",
                Type = ProjectType.Core,
                Divider = 40,
                DefaultDivider = 22500,
                Status = ScrappingStatus.InWaiting,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            }
        };
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var projectConfig in ProjectConfigs)
            {
                migrationBuilder.Sql($"INSERT INTO \"ProjectStats\" (\"ProjectName\", \"ProjectCategory\", \"ProjectStatisticUrl\", \"CountryStatisticUrl\", \"Type\", \"Divider\", \"DefaultDivider\", \"Status\", \"CreatedAt\", \"UpdatedAt\") " +
                                     $"VALUES ('{projectConfig.ProjectName}','{projectConfig.ProjectCategory}','{projectConfig.ProjectStatisticUrl}','{projectConfig.CountryStatisticUrl}','{(int)projectConfig.Type}','{projectConfig.Divider}','{projectConfig.DefaultDivider}','{(int)projectConfig.Status}', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var projectConfig in ProjectConfigs)
            {
                migrationBuilder.Sql($"DELETE FROM \"ProjectStats\" WHERE \"ProjectName\" = '{projectConfig.ProjectName}'");
            }
        }
    }
}
