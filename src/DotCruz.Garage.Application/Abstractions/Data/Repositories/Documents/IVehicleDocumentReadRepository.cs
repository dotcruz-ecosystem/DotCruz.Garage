using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Documents;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Documents;

public interface IVehicleDocumentReadRepository : IBaseReadRepository<VehicleDocument>
{
    Task<IReadOnlyCollection<VehicleDocument>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken);
    Task<(IReadOnlyCollection<VehicleDocument> Items, int TotalCount)> SearchAsync(
        int pageNumber,
        int pageSize,
        Guid? vehicleId,
        CancellationToken cancellationToken);
}
