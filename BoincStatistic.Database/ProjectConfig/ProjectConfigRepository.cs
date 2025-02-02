// using System;
// using System.Collections.Immutable;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
//
// namespace BoincStatistic.Database.ProjectConfig;
//
// public class ProjectConfigRepository : AbstractRepository<ProjectConfigModel>, IProjectConfigRepository
// {
//     public ProjectConfigRepository(PostgreSqlContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory)
//     {
//     }
//
//
//     public async Task<ProjectConfigModel> CreateModel(
//         string projectName,
//         string projectCategory,
//         string projectStatisticUrl,
//         string countryStatisticUrl,
//         ProjectType type,
//         int divider,
//         DateTimeOffset createdAt
//     )
//     {
//         var model = ProjectConfigModel.CreateModel(projectName, projectCategory, projectStatisticUrl, countryStatisticUrl, type, divider, createdAt);
//
//         var result = await CreateModelAsync(model);
//         if (result == null)
//         {
//             throw new Exception("Project Config Model is not create.");
//         }
//
//         return result;
//     }
//
//
//     public async Task<ImmutableArray<ProjectConfigModel>> List()
//     {
//         var collection = await DbModel.ToListAsync();
//
//         return [..collection];
//     }
// }