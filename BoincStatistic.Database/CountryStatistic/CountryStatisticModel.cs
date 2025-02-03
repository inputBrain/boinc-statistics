using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BoincStatistic.Database.ProjectStatistic;

namespace BoincStatistic.Database.CountryStatistic;

public class CountryStatisticModel : AbstractModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int ProjectId { get; set; }
    
    [ForeignKey("ProjectId")]
    public ProjectStatisticModel ProjectModel { get; set; }
    public string Rank { get; set; } 
    public string CountryName { get; set; }
    public string TotalCredit { get; set; } 
    public string CreditDay { get; set; }
    public string CreditWeek { get; set; }
    public string CreditMonth { get; set; }
    public string CreditAvarage { get; set; }
    public string CreditUser { get; set; }


    public static CountryStatisticModel CreateModel(int projectId, string rank, string countryName, string totalCredit, string creditDay, string creditWeek, string creditMonth, string creditAvarage, string creditUser)
    {
        return new CountryStatisticModel
        {
            ProjectId = projectId,
            Rank = rank,
            CountryName = countryName,
            TotalCredit = totalCredit,
            CreditDay = creditDay,
            CreditWeek = creditWeek,
            CreditMonth = creditMonth,
            CreditAvarage = creditAvarage,
            CreditUser = creditUser
        };
    }
}