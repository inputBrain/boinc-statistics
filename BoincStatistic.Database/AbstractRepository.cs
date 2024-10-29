using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BoincStatistic.Database;

public abstract class AbstractRepository<T> where T : AbstractModel
{
    protected readonly DbSet<T> DbModel;

    protected readonly ILogger<T> Logger;

    protected readonly PostgreSqlContext Context;


    protected AbstractRepository(PostgreSqlContext context, ILoggerFactory loggerFactory)
    {
        Context = context;
        DbModel = context.Set<T>();
        Logger = loggerFactory.CreateLogger<T>();
    }


    protected async Task<T> FindOne(int id)
    {
        var model = await DbModel.FindAsync(id);
        return model;
    }


    protected Task<List<T>> Find(Expression<Func<T, bool>> predicate)
    {
        return DbModel.Where(predicate).ToListAsync();
    }


    protected async Task<T> CreateModelAsync(T model)
    {
        await Context.AddAsync(model);
        var result = await Context.SaveChangesAsync();
        if (result == 0)
        {
            throw new Exception("Db error. Not Create any model");
        }

        return model;
    }


    protected Task<int> UpdateModelAsync(T model)
    {
        DbModel.Update(model);
        return Context.SaveChangesAsync();
    }

    
    protected async Task<ImmutableArray<T>> CreateBulkModelsAsync(ImmutableArray<T> collection)
    {
        await Context.AddRangeAsync(collection);
        var result = await Context.SaveChangesAsync();
        if (result == 0)
        {
            throw new Exception("Db error. Not Created Bulk models");
        }

        return collection;
    }
    
    
    protected async Task<ImmutableArray<T>> UpdateBulkModelsAsync(ImmutableArray<T> collection)
    {
        Context.UpdateRange(collection);
        var result = await Context.SaveChangesAsync();
        if (result == 0)
        {
            throw new Exception("Db error. Not Updated Bulk models");
        }

        return collection;
    }
}