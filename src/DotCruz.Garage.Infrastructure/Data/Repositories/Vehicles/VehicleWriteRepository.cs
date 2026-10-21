using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Vehicles;

public class VehicleWriteRepository(GarageDbContext context)
    : BaseWriteRepository<Vehicle>(context), IVehicleWriteRepository
{
    public async Task<Vehicle?> GetByIdWithHistoriesToUpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(v => v.OdometerHistories)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<Vehicle?> GetByIdWithHistoriesToUpdateUnscopedAsync(Guid id, Guid workspaceId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .Where(v => v.Id == id && v.WorkspaceId == workspaceId && v.DeletedAt == null)
            .Include(v => v.OdometerHistories)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Vehicle>> GetVehiclesWithStaleOdometerAsync(int staleDaysThreshold, CancellationToken cancellationToken)
    {
        var thresholdDate = DateTimeOffset.UtcNow.AddDays(-staleDaysThreshold);

        return await _dbSet
            .IgnoreQueryFilters()
            .Where(v => v.DeletedAt == null && (v.OdometerUpdatedAt == null || v.OdometerUpdatedAt <= thresholdDate))
            .ToListAsync(cancellationToken);
    }
}
