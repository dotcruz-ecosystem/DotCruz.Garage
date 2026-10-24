using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Vehicles;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;

public interface IVehicleReadRepository : IBaseReadRepository<Vehicle>
{
    Task<Vehicle?> GetByIdWithHistoriesAsync(Guid id, CancellationToken cancellationToken);
    Task<(IReadOnlyCollection<Vehicle> Items, int TotalCount)> SearchAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm,
        CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Vehicle>> GetVehiclesByAttributesAsync(
        string brand,
        string model,
        string engine,
        int? yearFrom,
        int? yearTo,
        CancellationToken cancellationToken);
}
