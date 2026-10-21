using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;

using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Maintenance;

public class MaintenanceLogWriteRepository(GarageDbContext context)
    : BaseWriteRepository<MaintenanceLog>(context), IMaintenanceLogWriteRepository
{
    public async Task<MaintenanceLog?> GetByIdWithAttachmentsToUpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(l => l.Attachments)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }
}
