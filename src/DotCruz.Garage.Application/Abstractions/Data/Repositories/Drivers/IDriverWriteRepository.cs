using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Drivers;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;

public interface IDriverWriteRepository : IBaseWriteRepository<Driver>
{
    Task<Driver?> GetByIdToUpdateWithAssignmentsAsync(Guid id, CancellationToken cancellationToken);
}
