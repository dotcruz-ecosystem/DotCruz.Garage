using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;

public interface IVehicleCatalogRepository
{
    Task<IReadOnlyCollection<string>> GetBrandsAsync(VehicleType? type, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<string>> GetModelsAsync(string brandName, VehicleType? type, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<string>> GetEnginesAsync(VehicleType? type, CancellationToken cancellationToken);
    Task<VehicleBrand> EnsureBrandExistsAsync(string brandName, CancellationToken cancellationToken);
    Task<VehicleModel> EnsureModelExistsAsync(Guid brandId, string modelName, VehicleType type, CancellationToken cancellationToken);
    Task<VehicleEngine> EnsureEngineExistsAsync(string engineName, VehicleType? type, CancellationToken cancellationToken);
}
