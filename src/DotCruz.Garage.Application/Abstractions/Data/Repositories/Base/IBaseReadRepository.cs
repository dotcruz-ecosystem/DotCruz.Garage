using DotCruz.Garage.Domain.Entities.Base;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;

public interface IBaseReadRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}
