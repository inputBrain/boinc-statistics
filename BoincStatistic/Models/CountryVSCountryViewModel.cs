namespace BoincStatistic.Models;

public class CountryVSCountryViewModel
{
    public List<ProjectWeightViewModel> Collection { get; set; } = [];
    
    public TotalSumModel TotalSumModel { get; set; }
}