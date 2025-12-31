using Microsoft.EntityFrameworkCore;

namespace VirtualWallet.Repositories;

public class AbstractBaseRepository<TEntity> where TEntity : class
{
    protected AppDbContext Context { get; }
    protected DbSet<TEntity> DbSet { get; }
    public AbstractBaseRepository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }
    
    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<List<TEntity>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public void AddAsync(TEntity entity)
    {
        DbSet.Add(entity);
    }

    public void UpdateAsync(TEntity entity)
    {
        DbSet.Update(entity);
    }
}