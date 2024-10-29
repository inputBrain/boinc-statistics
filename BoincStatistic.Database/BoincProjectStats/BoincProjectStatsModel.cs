using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoincStatistic.Database.BoincProjectStats;

public class BoincProjectStatsModel : AbstractModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string ProjectName { get; set; }
    
    public string ProjectCategory { get; set; }
    
    public string TotalCredit { get; set; }


    public static BoincProjectStatsModel CreateModel(string projectName, string projectCategory, string totalCredit)
    {
        return new BoincProjectStatsModel
        {
            ProjectName = projectName,
            ProjectCategory = projectCategory,
            TotalCredit = totalCredit
        };
    }
}