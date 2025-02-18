// namespace BoincStatistic;
//
// public class DataSeed
// {
//     using BoincStatistic.Database.ProjectConfig;
// using Microsoft.EntityFrameworkCore.Migrations;
//
// #nullable disable
//
// namespace BoincStatistic.Migrations
// {
//     /// <inheritdoc />
//     public partial class projectSeed : Migration
//     {
//         private List<ProjectConfigModel> ProjectConfigs = new List<ProjectConfigModel>()
//         {
//             new ProjectConfigModel()
//             {
//                 ProjectName = "GPUGRID",
//                 ProjectCategory = "Biology",
//                 ProjectStatisticUrl = "https://www.boincstats.com/stats/45/project/detail/",
//                 CountryStatisticUrl = "https://www.boincstats.com/stats/45/country/list",
//                 Type = ProjectType.GPU,
//                 Divider = 100000,
//                 Status = ScrappingStatus.Completed,
//                 CreatedAt = DateTimeOffset.Now,
//                 UpdatedAt = DateTimeOffset.Now
//             },
//             new ProjectConfigModel()
//             {
//                 ProjectName = "NumberFields",
//                 ProjectCategory = "Mathematics",
//                 ProjectStatisticUrl = "https://www.boincstats.com/stats/122/project/detail/",
//                 CountryStatisticUrl = "https://www.boincstats.com/stats/122/country/list",
//                 Type = ProjectType.GPU,
//                 Divider = 2000,
//                 Status = ScrappingStatus.Completed,
//                 CreatedAt = DateTimeOffset.Now,
//                 UpdatedAt = DateTimeOffset.Now
//             },
//             new ProjectConfigModel()
//             {
//                 ProjectName = "NFS",
//                 ProjectCategory = "Mathematics",
//                 ProjectStatisticUrl = "https://www.boincstats.com/stats/88/project/detail/",
//                 CountryStatisticUrl = "https://www.boincstats.com/stats/88/country/list",
//                 Type = ProjectType.Core,
//                 Divider = 90,
//                 Status = ScrappingStatus.Completed,
//                 CreatedAt = DateTimeOffset.Now,
//                 UpdatedAt = DateTimeOffset.Now
//             },
//             new ProjectConfigModel()
//             {
//                 ProjectName = "Total without ASIC",
//                 ProjectCategory = "Uncategorized",
//                 ProjectStatisticUrl = "https://www.boincstats.com/stats/-5/project/detail/",
//                 CountryStatisticUrl = "https://www.boincstats.com/stats/-5/country/list",
//                 Type = ProjectType.GPU,
//                 Divider = 22500,
//                 Status = ScrappingStatus.Completed,
//                 CreatedAt = DateTimeOffset.Now,
//                 UpdatedAt = DateTimeOffset.Now
//             },
//             new ProjectConfigModel()
//             {
//                 ProjectName = "Asteroids",
//                 ProjectCategory = "Astrophysics",
//                 ProjectStatisticUrl = "https://www.boincstats.com/stats/134/project/detail/",
//                 CountryStatisticUrl = "https://www.boincstats.com/stats/134/country/list",
//                 Type = ProjectType.Core,
//                 Divider = 45,
//                 Status = ScrappingStatus.Completed,
//                 CreatedAt = DateTimeOffset.Now,
//                 UpdatedAt = DateTimeOffset.Now
//             },
//             new ProjectConfigModel()
//             {
//                 ProjectName = "Climate Prediction",
//                 ProjectCategory = "Earth Sciences",
//                 ProjectStatisticUrl = "https://www.boincstats.com/stats/2/project/detail/",
//                 CountryStatisticUrl = "https://www.boincstats.com/stats/2/country/list",
//                 Type = ProjectType.Core,
//                 Divider = 70,
//                 Status = ScrappingStatus.Completed,
//                 CreatedAt = DateTimeOffset.Now,
//                 UpdatedAt = DateTimeOffset.Now
//             },
//             new ProjectConfigModel()
//             {
//                 ProjectName = "LODA",
//                 ProjectCategory = "Artificial intelligence",
//                 ProjectStatisticUrl = "https://www.boincstats.com/stats/199/project/detail/",
//                 CountryStatisticUrl = "https://www.boincstats.com/stats/199/country/list",
//                 Type = ProjectType.Core,
//                 Divider = 50,
//                 Status = ScrappingStatus.Completed,
//                 CreatedAt = DateTimeOffset.Now,
//                 UpdatedAt = DateTimeOffset.Now
//             },
//             new ProjectConfigModel()
//             {
//                 ProjectName = "Milkyway",
//                 ProjectCategory = "Astrophysics",
//                 ProjectStatisticUrl = "https://www.boincstats.com/stats/61/project/detail/",
//                 CountryStatisticUrl = "https://www.boincstats.com/stats/61/country/list",
//                 Type = ProjectType.Core,
//                 Divider = 50,
//                 Status = ScrappingStatus.Completed,
//                 CreatedAt = DateTimeOffset.Now,
//                 UpdatedAt = DateTimeOffset.Now
//             },
//             new ProjectConfigModel()
//             {
//                 ProjectName = "Rosetta",
//                 ProjectCategory = "Biology",
//                 ProjectStatisticUrl = "https://www.boincstats.com/stats/14/project/detail/",
//                 CountryStatisticUrl = "https://www.boincstats.com/stats/14/country/list",
//                 Type = ProjectType.Core,
//                 Divider = 40,
//                 Status = ScrappingStatus.Completed,
//                 CreatedAt = DateTimeOffset.Now,
//                 UpdatedAt = DateTimeOffset.Now
//             },
//             new ProjectConfigModel()
//             {
//                 ProjectName = "World Community Grid",
//                 ProjectCategory = "Umbrella project",
//                 ProjectStatisticUrl = "https://www.boincstats.com/stats/15/project/detail/",
//                 CountryStatisticUrl = "https://www.boincstats.com/stats/15/country/list",
//                 Type = ProjectType.Core,
//                 Divider = 50,
//                 Status = ScrappingStatus.Completed,
//                 CreatedAt = DateTimeOffset.Now,
//                 UpdatedAt = DateTimeOffset.Now
//             },
//             new ProjectConfigModel()
//             {
//                 ProjectName = "YAFU",
//                 ProjectCategory = "Mathematics",
//                 ProjectStatisticUrl = "https://www.boincstats.com/stats/121/project/detail/",
//                 CountryStatisticUrl = "https://www.boincstats.com/stats/121/country/list",
//                 Type = ProjectType.Core,
//                 Divider = 40,
//                 Status = ScrappingStatus.Completed,
//                 CreatedAt = DateTimeOffset.Now,
//                 UpdatedAt = DateTimeOffset.Now
//             },
//             new ProjectConfigModel()
//             {
//                 ProjectName = "Yoyo",
//                 ProjectCategory = "Umbrella project",
//                 ProjectStatisticUrl = "https://www.boincstats.com/stats/52/project/detail/",
//                 CountryStatisticUrl = "https://www.boincstats.com/stats/52/country/list",
//                 Type = ProjectType.Core,
//                 Divider = 40,
//                 Status = ScrappingStatus.Completed,
//                 CreatedAt = DateTimeOffset.Now,
//                 UpdatedAt = DateTimeOffset.Now
//             },
//             new ProjectConfigModel()
//             {
//                 ProjectName = "Amicable Numbers",
//                 ProjectCategory = "Mathematics",
//                 ProjectStatisticUrl = "https://www.boincstats.com/stats/172/project/detail/",
//                 CountryStatisticUrl = "https://www.boincstats.com/stats/172/country/list",
//                 Type = ProjectType.GPU,
//                 Divider = 55000,
//                 Status = ScrappingStatus.Completed,
//                 CreatedAt = DateTimeOffset.Now,
//                 UpdatedAt = DateTimeOffset.Now
//             },
//             new ProjectConfigModel()
//             {
//                 ProjectName = "Einstein",
//                 ProjectCategory = "Astrophysics",
//                 ProjectStatisticUrl = "https://www.boincstats.com/stats/5/project/detail/",
//                 CountryStatisticUrl = "https://www.boincstats.com/stats/5/country/list",
//                 Type = ProjectType.GPU,
//                 Divider = 20000,
//                 Status = ScrappingStatus.Completed,
//                 CreatedAt = DateTimeOffset.Now,
//                 UpdatedAt = DateTimeOffset.Now
//             },
//             new ProjectConfigModel()
//             {
//                 ProjectName = "Moo! Wrapper",
//                 ProjectCategory = "Mathematics",
//                 ProjectStatisticUrl = "https://www.boincstats.com/stats/114/project/detail/",
//                 CountryStatisticUrl = "https://www.boincstats.com/stats/114/country/list",
//                 Type = ProjectType.GPU,
//                 Divider = 45000,
//                 Status = ScrappingStatus.Completed,
//                 CreatedAt = DateTimeOffset.Now,
//                 UpdatedAt = DateTimeOffset.Now
//             },
//             new ProjectConfigModel()
//             {
//                 ProjectName = "PrimeGrid",
//                 ProjectCategory = "Mathematics",
//                 ProjectStatisticUrl = "https://www.boincstats.com/stats/11/project/detail/",
//                 CountryStatisticUrl = "https://www.boincstats.com/stats/11/country/list",
//                 Type = ProjectType.GPU,
//                 Divider = 7500,
//                 Status = ScrappingStatus.Completed,
//                 CreatedAt = DateTimeOffset.Now,
//                 UpdatedAt = DateTimeOffset.Now
//             },
//             new ProjectConfigModel()
//             {
//                 ProjectName = "LHC",
//                 ProjectCategory = "Physics",
//                 ProjectStatisticUrl = "https://www.boincstats.com/stats/3/project/detail/",
//                 CountryStatisticUrl = "https://www.boincstats.com/stats/3/country/list",
//                 Type = ProjectType.Core,
//                 Divider = 40,
//                 Status = ScrappingStatus.Completed,
//                 CreatedAt = DateTimeOffset.Now,
//                 UpdatedAt = DateTimeOffset.Now
//             }
//         };
//
//             
//             
//         /// <inheritdoc />
//         protected override void Up(MigrationBuilder migrationBuilder)
//         {
//
//         }
//
//         /// <inheritdoc />
//         protected override void Down(MigrationBuilder migrationBuilder)
//         {
//
//         }
//     }
// }
//
// }