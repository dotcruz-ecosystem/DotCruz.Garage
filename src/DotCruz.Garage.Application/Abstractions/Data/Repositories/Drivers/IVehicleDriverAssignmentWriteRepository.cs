using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Drivers;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;

public interface IVehicleDriverAssignmentWriteRepository : IBaseWriteRepository<VehicleDriverAssignment>
{
    Task<VehicleDriverAssignment?> GetCurrentByVehicleIdToUpdateAsync(Guid vehicleId, CancellationToken cancellationToken);
}
