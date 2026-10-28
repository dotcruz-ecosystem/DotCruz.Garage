using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Maintenance;

public class MaintenanceScheduleReadRepository(GarageDbContext context) 
    : BaseReadRepository<MaintenanceSchedule>(context), IMaintenanceScheduleReadRepository
{
    public async Task<IReadOnlyCollection<MaintenanceSchedule>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(s => s.Checkpoints)
            .Where(s => s.VehicleId == vehicleId)
            .OrderBy(s => s.NextDueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<MaintenanceSchedule>> GetPendingNotificationsAsync(CancellationToken cancellationToken)
    {
        // Fetches schedules that are Overdue or DueSoon
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.Status == ScheduleStatus.Overdue || s.Status == ScheduleStatus.DueSoon)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyCollection<MaintenanceSchedule> Items, int TotalCount)> SearchAsync(
        int pageNumber,
        int pageSize,
        Guid? vehicleId,
        CancellationToken cancellationToken)
    {
        var query = _dbSet.AsNoTracking();

        if (vehicleId.HasValue)
        {
            query = query.Where(s => s.VehicleId == vehicleId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Include(s => s.Checkpoints)
            .OrderBy(s => s.NextDueDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
