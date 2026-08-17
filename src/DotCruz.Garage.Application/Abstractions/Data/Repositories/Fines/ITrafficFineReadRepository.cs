using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Fines;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Fines;

public interface ITrafficFineReadRepository : IBaseReadRepository<TrafficFine>
{
    Task<IReadOnlyCollection<TrafficFine>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken);
    Task<(IReadOnlyCollection<TrafficFine> Items, int TotalCount)> SearchAsync(
        int pageNumber,
        int pageSize,
        Guid? vehicleId,
        CancellationToken cancellationToken);
}
