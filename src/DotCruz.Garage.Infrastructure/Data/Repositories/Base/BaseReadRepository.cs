using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Base;

public class BaseReadRepository<TEntity>(GarageDbContext context) : IBaseReadRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly GarageDbContext _context = context;
    protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.AsNoTracking().AnyAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }
}
