using Microsoft.EntityFrameworkCore;
using WildlifeConservation.Repositories.Data;

namespace WildlifeConservation.Repositories.Repositories;

public abstract class BaseRepository<TEntity>
    where TEntity : class, new()
{
    private readonly WildlifeDbContext dbContext;
    protected readonly DbSet<TEntity> DbSet;

    protected BaseRepository(WildlifeDbContext dbContext)
    {
        this.dbContext = dbContext;
        DbSet = dbContext.Set<TEntity>();
    }

    public IQueryable<TEntity> Query() => DbSet.AsNoTracking().AsQueryable();

    public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Query()
            .FirstOrDefaultAsync(x => EF.Property<int>(x, "Id") == id, cancellationToken);
    }

    public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = GetFlatValues(entity);
        DbSet.Add(dbEntity);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            DbSet.Entry(dbEntity).State = EntityState.Detached;
        }
        catch
        {
            DbSet.Remove(dbEntity);
            throw;
        }

        return dbEntity;
    }

    public async Task InsertRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var addedToContext = new List<TEntity>();

        foreach (var entity in entities)
        {
            var dbEntity = GetFlatValues(entity);
            DbSet.Add(dbEntity);
            addedToContext.Add(dbEntity);
        }

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);

            foreach (var entity in addedToContext)
            {
                DbSet.Entry(entity).State = EntityState.Detached;
            }
        }
        catch
        {
            DbSet.RemoveRange(addedToContext);
            throw;
        }
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        TEntity? addedToContext = null;

        try
        {
            var dbEntity = GetFlatValues(entity);
            DbSet.Attach(dbEntity);
            addedToContext = dbEntity;
            dbContext.Entry(dbEntity).State = EntityState.Modified;

            await dbContext.SaveChangesAsync(cancellationToken);
            return dbEntity;
        }
        finally
        {
            if (addedToContext is not null)
            {
                DbSet.Entry(addedToContext).State = EntityState.Detached;
            }
        }
    }

    public async Task UpdateRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var addedToContext = new List<TEntity>();

        try
        {
            foreach (var entity in entities)
            {
                var dbEntity = GetFlatValues(entity);
                DbSet.Attach(dbEntity);
                addedToContext.Add(dbEntity);
                dbContext.Entry(dbEntity).State = EntityState.Modified;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            foreach (var entity in addedToContext)
            {
                DbSet.Entry(entity).State = EntityState.Detached;
            }
        }
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        TEntity? addedToContext = null;

        try
        {
            var dbEntity = GetFlatValues(entity);
            DbSet.Attach(dbEntity);
            addedToContext = dbEntity;
            dbContext.Entry(dbEntity).State = EntityState.Deleted;

            await dbContext.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            if (addedToContext is not null)
            {
                DbSet.Entry(addedToContext).State = EntityState.Detached;
            }
        }
    }

    public async Task DeleteRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var addedToContext = new List<TEntity>();

        try
        {
            foreach (var entity in entities)
            {
                var dbEntity = GetFlatValues(entity);
                DbSet.Attach(dbEntity);
                addedToContext.Add(dbEntity);
                dbContext.Entry(dbEntity).State = EntityState.Deleted;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            foreach (var entity in addedToContext)
            {
                DbSet.Entry(entity).State = EntityState.Detached;
            }
        }
    }

    private TEntity GetFlatValues(TEntity source)
    {
        var target = new TEntity();
        var properties = DbSet.Entry(target).Properties;

        foreach (var propertyEntry in properties)
        {
            var property = propertyEntry.Metadata;
            if (property.IsShadowProperty())
            {
                continue;
            }

            propertyEntry.CurrentValue = property.GetGetter().GetClrValue(source);
        }

        return target;
    }
}
