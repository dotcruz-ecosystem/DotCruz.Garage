using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Maintenance;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;

public interface IMaintenanceLogWriteRepository : IBaseWriteRepository<MaintenanceLog>
{
    Task<MaintenanceLog?> GetByIdWithAttachmentsToUpdateAsync(Guid id, CancellationToken cancellationToken);
}
