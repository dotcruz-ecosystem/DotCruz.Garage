using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;

public interface IVehicleBlueprintRepository
{
    Task<VehicleBlueprint?> FindMatchingBlueprintAsync(
        string brand,
        string model,
        string engine,
        int year,
        VehicleType type,
        CancellationToken cancellationToken);

    Task<VehicleBlueprint?> GetByIdWithTemplatesAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<VehicleBlueprint>> GetPendingBlueprintsAsync(int limit, CancellationToken cancellationToken);
    Task AddAsync(VehicleBlueprint blueprint, CancellationToken cancellationToken);
}
