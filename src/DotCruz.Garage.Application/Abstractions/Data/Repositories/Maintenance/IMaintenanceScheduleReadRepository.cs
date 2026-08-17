using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Maintenance;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;

public interface IMaintenanceScheduleReadRepository : IBaseReadRepository<MaintenanceSchedule>
{
    Task<IReadOnlyCollection<MaintenanceSchedule>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<MaintenanceSchedule>> GetPendingNotificationsAsync(CancellationToken cancellationToken);
    Task<(IReadOnlyCollection<MaintenanceSchedule> Items, int TotalCount)> SearchAsync(
        int pageNumber,
        int pageSize,
        Guid? vehicleId,
        CancellationToken cancellationToken);
}
