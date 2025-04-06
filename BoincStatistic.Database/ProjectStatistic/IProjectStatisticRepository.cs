using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using BoincStatistic.Database.CountryStatistic;

namespace BoincStatistic.Database.ProjectStatistic;

public interface IProjectStatisticRepository
{
    public Task UpdateModel(ProjectStatisticModel model, string totalCredit, bool isCreditDayZero);

    Task UpdateUpdateAt(ProjectStatisticModel model, DateTimeOffset updatedAt);
    

    Task SetProjectStatus(ProjectStatisticModel model, ScrappingStatus scrappingStatus);

    Task SetToAllProjectsInWaitingStatus();

    Task<List<ProjectStatisticModel>> ListAll();
    
}