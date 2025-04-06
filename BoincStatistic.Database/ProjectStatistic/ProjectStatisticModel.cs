using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using BoincStatistic.Database.CountryStatistic;

namespace BoincStatistic.Database.ProjectStatistic;

public class ProjectStatisticModel : AbstractModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string ProjectName { get; set; }

    public string? DisplayName { get; set; }

    public string ProjectStatisticUrl { get; set; }

    public string CountryStatisticUrl { get; set; }

    public string? ProjectCategory { get; set; }

    public string? TotalCredit { get; set; }

    public ProjectType Type { get; set; }

    public int Divider { get; set; }
    
    public ScrappingStatus Status { get; set; }

    public bool IsScrappingActive { get; set; }
    
    public bool IsCreditDayZero { get; set; }

    public List<CountryStatisticModel> CountryStatistics { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }



    public static ProjectStatisticModel CreateModel(string projectName, string projectCategory, string totalCredit)
    {
        return new ProjectStatisticModel
        {
            ProjectName = projectName,
            ProjectCategory = projectCategory,
            TotalCredit = totalCredit,
            CountryStatistics = []
        };
    }


    public static bool IsSameTotalStatsModel(ProjectStatisticModel model, string totalCredit, bool isCreditDayZero)
    {
        return model.TotalCredit == totalCredit &&
               model.IsCreditDayZero == isCreditDayZero;
    }


    public void UpdateTotalStatsModel(ProjectStatisticModel model, string totalCredit, bool isCreditDayZero)
    {
        model.TotalCredit = totalCredit;
        model.IsCreditDayZero = isCreditDayZero;
    }


    public static bool IsSameDetailedStatistic(ProjectStatisticModel model, CountryStatisticModel apiModel)
    {

        var foundStats = model.CountryStatistics.FirstOrDefault(x => x.CountryName.Equals(apiModel.CountryName, StringComparison.CurrentCultureIgnoreCase));

        return foundStats?.Rank == apiModel.Rank &&
               foundStats.CountryName == apiModel.CountryName &&
               foundStats.TotalCredit == apiModel.TotalCredit &&
               foundStats.CreditDay == apiModel.CreditDay &&
               foundStats.CreditWeek == apiModel.CreditWeek &&
               foundStats.CreditMonth == apiModel.CreditMonth &&
               foundStats.CreditAvarage == apiModel.CreditAvarage &&
               foundStats.CreditUser == apiModel.CreditUser;
    }



    public void UpdateDetailedStatistics(ProjectStatisticModel model, CountryStatisticModel apiModel)
    {
        var foundStats = model.CountryStatistics.FirstOrDefault(x => x.CountryName.ToLower() == apiModel.CountryName.ToLower());
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