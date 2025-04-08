using System.Globalization;
using BoincStatistic.Database.ProjectStatistic;
using BoincStatistic.Models;

namespace BoincStatistic.Services;

public class CalculationService : ICalculationService
{
    private readonly IProjectStatisticRepository _projectStatisticRepository;


    public CalculationService(IProjectStatisticRepository projectStatisticRepository)
    {
        _projectStatisticRepository = projectStatisticRepository;
    }


    public async Task<List<ProjectWeightViewModel>> BaseCalculationByDefaultUAvsRuAsync(string firstCountryName = "Ukraine", string secondCountryName = "Russian Federation", string? rank = null)
    {
        var projectOverviewList = new List<ProjectWeightViewModel>();

        var projectList = await _projectStatisticRepository.ListAll();

        foreach (var project in projectList)
        {
            var countryFirstByDefaultUa = project.CountryStatistics.FirstOrDefault(x => x.CountryName == firstCountryName);

            var countrySecondByDefaultRu = project.CountryStatistics.FirstOrDefault(x => x.CountryName == secondCountryName);
            if (rank != null)
            {
                countrySecondByDefaultRu = project.CountryStatistics.FirstOrDefault(x => x.Rank == rank);
            }

            
            if (countryFirstByDefaultUa == null || countrySecondByDefaultRu == null)
            {
                continue;
            }

            var uaCredit = decimal.Parse(countryFirstByDefaultUa.TotalCredit.Replace(",", ""));
            var ruCredit = decimal.Parse(countrySecondByDefaultRu.TotalCredit.Replace(",", ""));

            var totalCredit = decimal.Parse(project.TotalCredit.Replace(",", ""));

            var uaAverage = decimal.Parse(countryFirstByDefaultUa.CreditAvarage.Replace(",", ""));
            var ruAverage = decimal.Parse(countrySecondByDefaultRu.CreditAvarage.Replace(",", ""));
            

            var uaWeight = Math.Round((uaCredit / totalCredit) * 100, 2);
            var ruWeight = Math.Round((ruCredit / totalCredit) * 100, 2);
            
            var creditDifference = ruCredit - uaCredit;

            var foundDaysToWinWord = string.Empty;
            var daysToWinWithMinus = string.Empty;
            if (creditDifference < 0)
            {
                if (ruAverage == 0)
                {
                    ruAverage = 1;
                }
                var copyDifference = creditDifference / ruAverage;
                
                daysToWinWithMinus = Math.Round(creditDifference / ruAverage, 1).ToString(CultureInfo.InvariantCulture);
                foundDaysToWinWord = _getDaysToWinCategory((double)copyDifference);
            }
            
            var creditsPerHour = project.Divider;
            var projectType = project.Type;
            var taskHours = Math.Round(creditDifference / creditsPerHour, 0);

            var yearsDifference = Math.Round(taskHours / 8760, 2);

            var mwthMultiplier = project.Type == ProjectType.GPU ? 150 : 7;
            
            var mwth = Math.Ceiling(taskHours * mwthMultiplier / 1000000 * 100) / 100;
            
            var daysToWin = creditDifference > 0 && uaAverage > ruAverage ? Math.Round(creditDifference / (uaAverage - ruAverage), 0) + 1 : 0;

            var devicesToOvercome = Math.Round((ruAverage - uaAverage) / (creditsPerHour * 24), 0) + 1;

            var daysToWinAsString = daysToWin.ToString(CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(foundDaysToWinWord) == false)
            {
                daysToWinAsString = foundDaysToWinWord;
            }
            
            var isProjectNotWorking = false;

            var hasUkraineCreditDayZero = project.CountryStatistics.First(x => x.CountryName == "Ukraine").CreditDay == "0";
            var isSinceThenZero = project.IsCreditDayZero;

            if (hasUkraineCreditDayZero && isSinceThenZero)
            {
                isProjectNotWorking = true;
            }
            
            if (isProjectNotWorking)
            {
                daysToWinAsString = "0";
            }

            projectOverviewList.Add(new ProjectWeightViewModel {
                Country = rank != null ? countrySecondByDefaultRu.CountryName : "",
                ProjectName = project.DisplayName ?? project.ProjectName,
                ProjectStatsUrl = project.ProjectStatisticUrl,
                CountryStatsUrl = project.CountryStatisticUrl,
                UaWeight = (double)uaWeight,
                RuWeight = (double)ruWeight,
                CreditDifference = Math.Round((double)creditDifference, 2),
                CreditUA = countryFirstByDefaultUa.TotalCredit,
                CreditRU = countrySecondByDefaultRu.TotalCredit,
                AvarageRU = countrySecondByDefaultRu.CreditAvarage,
                AvarageUA = countryFirstByDefaultUa.CreditAvarage,
                TaskHours = (double)taskHours,
                YearsDifference = (double)yearsDifference,
                MWtPerHourCpu = (double)mwth,
                DevicesToOvercome = (double)devicesToOvercome,
                DaysToWin = daysToWinAsString,
                ProjectType = projectType == ProjectType.GPU ? "GPU" : "Core",
                IsCreditDayZero = isProjectNotWorking,
                DaysToWinWithMinus = daysToWinWithMinus
            });
        }

        return projectOverviewList;
    }


    public TotalScoreViewModel CalculateTotalsUaAndRuByProjectType(List<ProjectWeightViewModel> projects, string projectType)
    {
        var totals = new TotalScoreViewModel { ProjectType = projectType };
        
        totals.TotalTaskHours = Math.Round(projects.Sum(p => p.TaskHours), 2);
        totals.TotalYearsDifference = Math.Round(projects.Sum(p => p.YearsDifference), 2);
        totals.TotalMWtPerHourCpu = Math.Round(projects.Sum(p => p.MWtPerHourCpu), 2);
        totals.TotalDevicesToOvercome = Math.Round(projects.Sum(p => p.DevicesToOvercome), 2);
        
        var creditUa = projects.Sum(p => decimal.Parse(p.CreditUA.Replace(",", ""), CultureInfo.InvariantCulture));
        var creditRu = projects.Sum(p => decimal.Parse(p.CreditRU.Replace(",", ""), CultureInfo.InvariantCulture));
        var avarageUa = projects.Sum(p => decimal.Parse(p.AvarageUA.Replace(",", ""), CultureInfo.InvariantCulture));
        var avarageRu = projects.Sum(p => decimal.Parse(p.AvarageRU.Replace(",", ""), CultureInfo.InvariantCulture));


        totals.TotalCreditUA = creditUa.ToString("N0", CultureInfo.InvariantCulture);
        totals.TotalCreditRU = creditRu.ToString("N0", CultureInfo.InvariantCulture);
        totals.TotalAvarageUA = avarageUa.ToString("N0", CultureInfo.InvariantCulture);
        totals.TotalAvarageRU = avarageRu.ToString("N0", CultureInfo.InvariantCulture);
        
        var totalCreditDiff = totals.TotalCreditDifference = Math.Round(projects.Sum(p => p.CreditDifference), 2);
        
        var daysToWin = totalCreditDiff > 0 && avarageUa > avarageRu ? Math.Round(totalCreditDiff / (double) (avarageUa - avarageRu), 0) + 1 : 0;

        var foundDaysToWinWord = string.Empty;
        if (totalCreditDiff < 0)
        {
            var copyDifference = totalCreditDiff / (double) avarageRu;
            foundDaysToWinWord = _getDaysToWinCategory(copyDifference);
        }
        
        var daysToWinAsString = daysToWin.ToString(CultureInfo.InvariantCulture);

        if (string.IsNullOrEmpty(foundDaysToWinWord) == false)
        {
            daysToWinAsString = foundDaysToWinWord;
        }
        
        totals.TotalDaysToWin = daysToWinAsString;
        return totals;
    }


    private string _getDaysToWinCategory(double daysToWin)
    {
        var roundedDaysToWin = (int)Math.Round(daysToWin);

        if (roundedDaysToWin >= -100 && roundedDaysToWin <= 0)
            return "Overcome";
        else if (roundedDaysToWin >= -365 && roundedDaysToWin <= -101)
            return "Won";
        else if (roundedDaysToWin >= -999 && roundedDaysToWin <= -366)
            return "Ownage";
        else if (roundedDaysToWin >= -10000 && roundedDaysToWin <= -1000)
            return "Destroyed";
        else if (roundedDaysToWin < -10000)
            return "Annihilated";
        else
            return roundedDaysToWin.ToString();
    }
}