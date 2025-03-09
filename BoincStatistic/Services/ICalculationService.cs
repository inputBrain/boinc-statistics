using BoincStatistic.Models;

namespace BoincStatistic.Services;

public interface ICalculationService
{
    Task<List<ProjectWeightViewModel>> BaseCalculationByDefaultUAvsRuAsync(string firstCountryName = "Ukraine", string secondCountryName = "Russian Federation", string? rank = null);

    TotalScoreViewModel CalculateTotalsUaAndRuByProjectType(List<ProjectWeightViewModel> projects, string projectType);
}