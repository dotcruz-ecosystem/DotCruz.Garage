using DotCruz.Garage.Application.Abstractions.Data.Repositories.ServiceOrders;
using DotCruz.Garage.Domain.Entities.ServiceOrders;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.ServiceOrders;

public class ServiceOrderReadRepository(GarageDbContext context)
    : BaseReadRepository<ServiceOrder>(context), IServiceOrderReadRepository
{
    public async Task<ServiceOrder?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(so => so.Items)
            .Include(so => so.Attachments)
            .FirstOrDefaultAsync(so => so.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyCollection<ServiceOrder> Items, int TotalCount)> GetByVehicleIdPaginatedAsync(
        Guid vehicleId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(so => so.Items)
            .Include(so => so.Attachments)
            .Where(so => so.VehicleId == vehicleId)
            .OrderByDescending(so => so.PerformedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyCollection<ServiceOrder>> GetByVehicleIdAsync(
        Guid vehicleId,
        CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(so => so.Items)
            .Include(so => so.Attachments)
            .Where(so => so.VehicleId == vehicleId)
            .OrderByDescending(so => so.PerformedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ServiceOrder>> GetByVehicleIdUnscopedAsync(
        Guid vehicleId,
        Guid workspaceId,
        CancellationToken cancellationToken)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(so => so.Items)
            .Include(so => so.Attachments)
            .Where(so => so.VehicleId == vehicleId && (so.OriginWorkspaceId == workspaceId || workspaceId == Guid.Empty) && so.DeletedAt == null)
            .OrderByDescending(so => so.PerformedAt)
            .ToListAsync(cancellationToken);
    }
}
