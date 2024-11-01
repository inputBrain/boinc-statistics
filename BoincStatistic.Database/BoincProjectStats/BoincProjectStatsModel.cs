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
    
    
    public static bool IsSameTotalStatsModel(BoincProjectStatsModel model, string projectName, string projectCategory, string totalCredit)
    {
        return model.ProjectName == projectName &&
               model.ProjectCategory == projectCategory &&
               model.TotalCredit == totalCredit;
    }
    
    
    public void UpdateTotalStatsModel(BoincProjectStatsModel model, string projectName, string projectCategory, string totalCredit)
    {
        model.ProjectName = projectName;
        model.ProjectCategory = projectCategory;
        model.TotalCredit = totalCredit;
    }
    
    
    

    
    public static bool IsSameDetailedStatistic(BoincProjectStatsModel model, BoincStatsModel apiModel)
    {
    
        var foundStats = model.DetailedStatistics.FirstOrDefault(x => x.CountryName.ToLower() == apiModel.CountryName.ToLower());

        return foundStats?.Rank == apiModel.Rank &&
               foundStats.CountryName == apiModel.CountryName &&
               foundStats.TotalCredit == apiModel.TotalCredit &&
               foundStats.CreditDay == apiModel.CreditDay &&
               foundStats.CreditWeek == apiModel.CreditWeek &&
               foundStats.CreditMonth == apiModel.CreditMonth &&
               foundStats.CreditAvarage == apiModel.CreditAvarage &&
               foundStats.CreditUser == apiModel.CreditUser;
    }

    
    
    public void UpdateDetailedStatistics(BoincProjectStatsModel model, BoincStatsModel apiModel)
    {
        var foundStats = model.DetailedStatistics.FirstOrDefault(x => x.CountryName.ToLower() == apiModel.CountryName.ToLower());
        if (foundStats == null)
        {
            return;
        }
        foundStats.Rank = apiModel.Rank;
        foundStats.CountryName = apiModel.CountryName;
        foundStats.TotalCredit = apiModel.TotalCredit;
        foundStats.CreditDay = apiModel.CreditDay;
        foundStats.CreditWeek = apiModel.CreditWeek;
        foundStats.CreditMonth = apiModel.CreditMonth;
        foundStats.CreditAvarage = apiModel.CreditAvarage;
        foundStats.CreditUser = apiModel.CreditUser;
    }
}