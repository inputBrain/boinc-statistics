// using System;
// using System.Collections.Immutable;
// using System.Threading.Tasks;
//
// namespace BoincStatistic.Database.ProjectConfig;
//
// public interface IProjectConfigRepository
// {
//     Task<ProjectConfigModel> CreateModel(
//         string projectName,
//         string projectCategory,
//         string projectStatisticUrl,
//         string countryStatisticUrl,
//         ProjectType type,
//         int divider,
//         DateTimeOffset createdAt
//     );
//     
//     Task<ImmutableArray<ProjectConfigModel>> List();
// }