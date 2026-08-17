using DotCruz.Garage.Application.Abstractions.Data.Repositories.Fines;
using DotCruz.Garage.Domain.Entities.Fines;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Fines;

public class TrafficFineWriteRepository(GarageDbContext context)
    : BaseWriteRepository<TrafficFine>(context), ITrafficFineWriteRepository
{
    public async Task<IReadOnlyCollection<TrafficFine>> GetPendingFinesToNotifyAsync(CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        return await _dbSet
            .IgnoreQueryFilters()
            .Where(tf => tf.DeletedAt == null && tf.Vehicle.DeletedAt == null)
            .Include(tf => tf.Vehicle)
            .Where(tf => tf.Status == FineStatus.Pending 
                && tf.LastNotifiedAt == null 
                && tf.DueDate <= now.AddDays(tf.WarningThresholdDays ?? TrafficFine.DefaultWarningThresholdDays))
            .ToListAsync(cancellationToken);
    }
}
