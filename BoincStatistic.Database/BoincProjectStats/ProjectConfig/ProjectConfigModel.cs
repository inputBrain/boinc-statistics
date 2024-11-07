using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoincStatistic.Database.ProjectConfig;

public class ProjectConfigModel : AbstractModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string ProjectStatsUrl { get; set; }
    
    public string FullCountryStatsUrl { get; set; }
    
    public ProjectType Type { get; set; }
    
    public int Divider { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
}