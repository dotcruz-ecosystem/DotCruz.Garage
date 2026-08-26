using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Drivers;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;

public interface IVehicleDriverAssignmentReadRepository : IBaseReadRepository<VehicleDriverAssignment>
{
    Task<IReadOnlyCollection<VehicleDriverAssignment>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<VehicleDriverAssignment>> GetByDriverIdAsync(Guid driverId, CancellationToken cancellationToken);
    Task<VehicleDriverAssignment?> GetCurrentByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken);
}
