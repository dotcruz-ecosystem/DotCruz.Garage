using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Vehicles;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;

public interface IVehicleSaleWriteRepository : IBaseWriteRepository<VehicleSale>
{
    Task<VehicleSale?> GetActiveSaleByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken);
    Task<VehicleSale?> GetSaleToUpdateUnscopedAsync(Guid saleId, Guid workspaceId, CancellationToken cancellationToken);
}
