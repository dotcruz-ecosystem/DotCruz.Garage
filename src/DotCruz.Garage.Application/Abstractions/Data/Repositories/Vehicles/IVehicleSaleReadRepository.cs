using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Vehicles;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;

public interface IVehicleSaleReadRepository : IBaseReadRepository<VehicleSale>
{
    Task<IReadOnlyCollection<VehicleSale>> GetActiveSalesWithVehicleAsync(CancellationToken cancellationToken);
    Task<VehicleSale?> GetByIdWithVehicleAsync(Guid id, CancellationToken cancellationToken);
    Task<VehicleSale?> GetByVehicleIdWithVehicleAsync(Guid vehicleId, CancellationToken cancellationToken);
    Task<VehicleSale?> GetSaleUnscopedAsync(Guid saleId, Guid tenantId, CancellationToken cancellationToken);
}
