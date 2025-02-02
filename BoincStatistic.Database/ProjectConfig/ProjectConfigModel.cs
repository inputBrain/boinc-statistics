// using System;
// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
//
// namespace BoincStatistic.Database.ProjectConfig;
//
// public class ProjectConfigModel : AbstractModel
// {
//     [Key]
//     [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//     public int Id { get; set; }
//
//     public string ProjectName { get; set; }
//
//     public string ProjectCategory { get; set; }
//
//     public string ProjectStatisticUrl { get; set; }
//
//     public string CountryStatisticUrl { get; set; }
//
//     public ProjectType Type { get; set; }
//     public int Divider { get; set; }
//
//     public ScrappingStatus Status { get; set; }
//
//     public DateTimeOffset CreatedAt { get; set; }
//
//     public DateTimeOffset UpdatedAt { get; set; }
//
//
//     public static ProjectConfigModel CreateModel(
//         string projectName,
//         string projectCategory,
//         string projectStatisticUrl,
//         string countryStatisticUrl,
//         ProjectType type,
//         int divider,
//         DateTimeOffset createdAt
//     )
//     {
//         return new ProjectConfigModel
//         {
//             ProjectName = projectName,
//             ProjectCategory = projectCategory,
//             ProjectStatisticUrl = projectStatisticUrl,
//             CountryStatisticUrl = countryStatisticUrl,
//             Type = type,
//             Divider = divider,
//             CreatedAt = createdAt
//         };
//     }
// }