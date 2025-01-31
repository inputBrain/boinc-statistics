﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BoincStatistic.Database.BoincStats;

public class BoincStatsRepository : AbstractRepository<BoincStatsModel>, IBoincStatsRepository
{
    public BoincStatsRepository(PostgreSqlContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory)
    {
    }


    public async Task<BoincStatsModel> CreateModel(
        int projectId,
        string rank,
        string countryName,
        string totalCredit,
        string creditDay,
        string creditWeek,
        string creditMonth,
        string creditAverage,
        string creditUser
    )
    {
        var model = BoincStatsModel.CreateModel(projectId, rank, countryName, totalCredit, creditDay, creditWeek, creditMonth, creditAverage, creditUser);

        var result = await CreateModelAsync(model);
        if (result == null)
        {
            throw new Exception("Boinc stats model is not created");
        }

        return result;
    }


    public async Task<BoincStatsModel> GetOneCountryStatsByCountryName(string country, int projectId)
    {
        var model = await DbModel
            .Where(x => x.CountryName.ToLower() == country.ToLower() && x.ProjectId == projectId)
            .FirstOrDefaultAsync();
        
        if (model == null)
        {
            throw new Exception("Country stats is not found");
        }

        return model;
    }
    
    
    public async Task<BoincStatsModel> GetOneByRank(string rank, int projectId)
    {
        var model =  await DbModel
                                .Where(x => x.Rank == rank && x.ProjectId == projectId)
                                .FirstOrDefaultAsync();
        return model;
    }
    

    public async Task<List<BoincStatsModel>> ListAllAsync()
    {
        return await DbModel.OrderBy(x => x.Id).ToListAsync();
    }
    
    
    public async Task<int> CountAsync()
    {
        return await DbModel.CountAsync();
    }

    
    public async Task<List<BoincStatsModel>> GetThreeCountryAsync(int projectId)
    {
        return await DbModel
            .OrderBy(b => b.Id)
            .Where(x => x.ProjectId == projectId)
            .Where(x => x.Rank == "1" || x.CountryName == "Ukraine" || x.CountryName == "Russian Federation")
            .Take(3)
            .ToListAsync();
    }
    


    
    
    public async Task<List<BoincStatsModel>> GetPaginatedAsync(int pageNumber, int pageSize)
    {
        return await DbModel
            .OrderBy(b => b.Id)
            .Where(x => x.Rank == "1" || x.CountryName == "Ukraine" || x.CountryName == "Russian Federation")
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}