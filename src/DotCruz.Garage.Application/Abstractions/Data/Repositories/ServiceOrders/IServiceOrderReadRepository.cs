using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.ServiceOrders;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.ServiceOrders;

public interface IServiceOrderReadRepository : IBaseReadRepository<ServiceOrder>
{
    Task<ServiceOrder?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken);

    Task<(IReadOnlyCollection<ServiceOrder> Items, int TotalCount)> GetByVehicleIdPaginatedAsync(
        Guid vehicleId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ServiceOrder>> GetByVehicleIdAsync(
        Guid vehicleId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ServiceOrder>> GetByVehicleIdUnscopedAsync(
        Guid vehicleId,
        Guid workspaceId,
        CancellationToken cancellationToken);
}
