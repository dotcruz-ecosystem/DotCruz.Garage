using DotCruz.Garage.Application.Abstractions.Data.Repositories.Documents;
using DotCruz.Garage.Domain.Entities.Documents;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Documents;

public class VehicleDocumentWriteRepository(GarageDbContext context)
    : BaseWriteRepository<VehicleDocument>(context), IVehicleDocumentWriteRepository
{
    public async Task<IReadOnlyCollection<VehicleDocument>> GetDocumentsToNotifyAsync(CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        return await _dbSet
            .Include(d => d.Vehicle)
            .Where(d => 
                (d.Status == DocumentStatus.Pending && d.DueDate <= now.AddDays(d.WarningThresholdDays ?? VehicleDocument.DefaultWarningThresholdDays) && (d.LastNotifiedStatus != DocumentStatus.Pending || d.LastNotifiedAt == null))
                ||
                (d.Status == DocumentStatus.Paid && d.ExpirationDate <= now.AddDays(d.WarningThresholdDays ?? VehicleDocument.DefaultWarningThresholdDays) && (d.LastNotifiedStatus != DocumentStatus.Expired || d.LastNotifiedAt == null))
            )
            .ToListAsync(cancellationToken);
    }
}
