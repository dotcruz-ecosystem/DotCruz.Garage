using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Maintenance;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;

public interface IMaintenanceLogReadRepository : IBaseReadRepository<MaintenanceLog>
{
    Task<(IReadOnlyCollection<MaintenanceLog> Items, int TotalCount)> GetByVehicleIdPaginatedAsync(
        Guid vehicleId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<MaintenanceLog>> GetByVehicleIdUnscopedAsync(
        Guid vehicleId,
        Guid tenantId,
        CancellationToken cancellationToken);
}
