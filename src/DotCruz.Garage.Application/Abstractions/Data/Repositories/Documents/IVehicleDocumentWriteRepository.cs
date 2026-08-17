using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Documents;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Documents;

public interface IVehicleDocumentWriteRepository : IBaseWriteRepository<VehicleDocument>
{
    Task<IReadOnlyCollection<VehicleDocument>> GetDocumentsToNotifyAsync(CancellationToken cancellationToken);
}
