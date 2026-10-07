using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Vehicles;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;

public interface IVehicleWriteRepository : IBaseWriteRepository<Vehicle>
{
    Task<Vehicle?> GetByIdWithHistoriesToUpdateAsync(Guid id, CancellationToken cancellationToken);
    Task<Vehicle?> GetByIdWithHistoriesToUpdateUnscopedAsync(Guid id, Guid workspaceId, CancellationToken cancellationToken);
    Task<Vehicle?> GetByIdWithSalesToUpdateAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Vehicle>> GetVehiclesWithStaleOdometerAsync(int staleDaysThreshold, CancellationToken cancellationToken);
}
