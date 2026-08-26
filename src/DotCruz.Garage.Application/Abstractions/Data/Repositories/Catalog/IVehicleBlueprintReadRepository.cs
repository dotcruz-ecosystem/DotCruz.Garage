using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Catalog;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Catalog;

public interface IVehicleBlueprintReadRepository : IBaseReadRepository<VehicleBlueprint>
{
    Task<VehicleBlueprint?> GetByDetailsWithTemplatesAsync(
        string brand,
        string model,
        int year,
        string? engine,
        CancellationToken cancellationToken);

    Task<VehicleBlueprint?> GetByIdWithTemplatesAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<(IReadOnlyCollection<VehicleBlueprint> Items, int TotalCount)> SearchAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm,
        string? brand,
        string? model,
        int? year,
        CancellationToken cancellationToken);
}
