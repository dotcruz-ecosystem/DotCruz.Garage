using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Maintenance;

public class MaintenanceLogReadRepository(GarageDbContext context) 
    : BaseReadRepository<MaintenanceLog>(context), IMaintenanceLogReadRepository
{
    public async Task<(IReadOnlyCollection<MaintenanceLog> Items, int TotalCount)> GetByVehicleIdPaginatedAsync(
        Guid vehicleId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(l => l.VehicleId == vehicleId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(l => l.PerformedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyCollection<MaintenanceLog>> GetByVehicleIdUnscopedAsync(
        Guid vehicleId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(l => l.VehicleId == vehicleId && l.TenantId == tenantId && l.DeletedAt == null)
            .OrderByDescending(l => l.PerformedAt)
            .ToListAsync(cancellationToken);
    }
}
