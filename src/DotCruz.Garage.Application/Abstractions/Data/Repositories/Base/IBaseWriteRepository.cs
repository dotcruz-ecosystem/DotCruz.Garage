using DotCruz.Garage.Domain.Entities.Base;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;

public interface IBaseWriteRepository<TEntity> where TEntity : BaseEntity
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken);
    Task<TEntity?> GetByIdToUpdateAsync(Guid id, CancellationToken cancellationToken);
}
