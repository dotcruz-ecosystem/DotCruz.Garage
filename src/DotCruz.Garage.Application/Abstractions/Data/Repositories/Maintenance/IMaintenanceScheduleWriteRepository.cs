using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Maintenance;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;

public interface IMaintenanceScheduleWriteRepository : IBaseWriteRepository<MaintenanceSchedule>
{
    Task<IReadOnlyCollection<MaintenanceSchedule>> GetByVehicleIdToUpdateAsync(Guid vehicleId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<MaintenanceSchedule>> GetSchedulesWithVehicleDetailsToUpdateAsync(CancellationToken cancellationToken);
}
