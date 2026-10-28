using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Base;

public class BaseWriteRepository<TEntity> : IBaseWriteRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly GarageDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public BaseWriteRepository(GarageDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual async Task<TEntity?> GetByIdToUpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
}
