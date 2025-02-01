namespace BoincStatistic.Models;

public class ProjectsSimpleViewModel
{
    public string ProjectName { get; set; }
    
    public string TotalCredit { get; set; }
    
    public string Category { get; set; }
    
    public bool HasMoreThanZeroCreditDay { get; set; }
}