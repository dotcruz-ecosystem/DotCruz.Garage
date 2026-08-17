using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Accessories;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Accessories;

public interface IVehicleAccessoryReadRepository : IBaseReadRepository<VehicleAccessory>
{
    Task<IReadOnlyCollection<VehicleAccessory>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken);
    Task<(IReadOnlyCollection<VehicleAccessory> Items, int TotalCount)> SearchAsync(
        int pageNumber,
        int pageSize,
        Guid? vehicleId,
        CancellationToken cancellationToken);
}
