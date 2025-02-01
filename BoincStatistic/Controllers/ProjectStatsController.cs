using BoincStatistic.Database.CountryStatistic;
using BoincStatistic.Database.ProjectStatistic;
using BoincStatistic.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoincStatistic.Controllers;

public class ProjectStatsController : Controller
{
    private readonly ILogger<ProjectStatsController> _logger;

    private readonly IProjectStatisticRepository _projectStatisticRepository;
    private readonly ICountryStatisticRepository _countryStatistic;


    public ProjectStatsController(ILogger<ProjectStatsController> logger, IProjectStatisticRepository projectStatisticRepository, ICountryStatisticRepository countryStatistic)
    {
        _logger = logger;
        _projectStatisticRepository = projectStatisticRepository;
        _countryStatistic = countryStatistic;
    }
    
    [Route("projects")]
    public async Task<IActionResult> Index()
    {
        // var totalRecords = await _projectStatisticRepository.CountAsync();
        // var boincStats = await _projectStatisticRepository.GetPaginatedAsync(pageNumber, pageSize);

        var collection = await _projectStatisticRepository.ListAll();

        var viewCollection = new List<ProjectsSimpleViewModel>();

        foreach (var project in collection)
        {
            var hasMoreThanZeroCreditDay = await _countryStatistic.CountCreditDayRows(project.Id);

            
            viewCollection.Add(new ProjectsSimpleViewModel{
                ProjectName = project.ProjectName, 
                TotalCredit = project.TotalCredit,
                Category = project.ProjectCategory, 
                HasMoreThanZeroCreditDay = hasMoreThanZeroCreditDay}
            );
        }
        
        // var model = new BoincProjectStatsViewModel
        // {
        //     ProjectStats = boincStats,
        //     PageNumber = pageNumber,
        //     PageSize = pageSize,
        //     TotalRecords = totalRecords
        // };

        return View(viewCollection);
    }

}