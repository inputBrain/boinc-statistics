using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using BoincStatistic.Database.BoincStats;

namespace BoincStatistic.Database.BoincProjectStats;

public class BoincProjectStatsModel : AbstractModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string ProjectName { get; set; }
    
    public string ProjectCategory { get; set; }
    
    public string TotalCredit { get; set; }
    
    public List<BoincStatsModel> DetailedStatistics { get; set; }


    public static BoincProjectStatsModel CreateModel(string projectName, string projectCategory, string totalCredit)
    {
        return new BoincProjectStatsModel
        {
            ProjectName = projectName,
            ProjectCategory = projectCategory,
            TotalCredit = totalCredit,
            DetailedStatistics = []
        };
    }


    // public static bool IsSameDetailedStatistic(BoincProjectStatsModel model, List<BoincStatsModel> detailedStatistic)
    // {
    //     if (model.DetailedStatistics.Count != detailedStatistic.Count)
    //     {
    //         return false;
    //     }
    //
    //     var sortedStatistics = model.DetailedStatistics.OrderBy(x => x.CountryName).ToList();
    //     var sortedApiStatistics = detailedStatistic.OrderBy(x => x.CountryName).ToList();
    //
    //
    //     foreach (var statistic in sortedStatistics)
    //     {
    //         foreach (var apiStatistic in sortedApiStatistics)
    //         {
    //             return statistic.CountryName
    //         }
    //     }
    //     
    //     
    // }
    
    
}