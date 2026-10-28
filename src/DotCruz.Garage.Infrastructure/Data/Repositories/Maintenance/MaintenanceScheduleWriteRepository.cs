using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Maintenance;

public class MaintenanceScheduleWriteRepository(GarageDbContext context)
    : BaseWriteRepository<MaintenanceSchedule>(context), IMaintenanceScheduleWriteRepository
{
    public async Task<MaintenanceSchedule?> GetByIdWithCheckpointsToUpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(s => s.Checkpoints)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<MaintenanceSchedule>> GetByVehicleIdToUpdateAsync(Guid vehicleId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(s => s.Checkpoints)
            .Where(s => s.VehicleId == vehicleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<MaintenanceSchedule>> GetByVehicleIdToUpdateUnscopedAsync(Guid vehicleId, Guid workspaceId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .Where(s => s.VehicleId == vehicleId && (s.OriginWorkspaceId == workspaceId || workspaceId == Guid.Empty) && s.DeletedAt == null)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<MaintenanceSchedule>> GetSchedulesWithVehicleDetailsToUpdateAsync(CancellationToken cancellationToken)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .Where(ms => ms.DeletedAt == null && ms.Vehicle.DeletedAt == null)
            .Include(ms => ms.Vehicle)
                .ThenInclude(v => v.Workspaces)
            .Where(ms =>
                (ms.NextDueOdometer.HasValue && (ms.NextDueOdometer.Value - ms.Vehicle.CurrentOdometer) <= (ms.WarningThresholdOdometer ?? MaintenanceSchedule.DefaultWarningThresholdOdometer)) ||
                (ms.NextDueDate.HasValue && (ms.NextDueDate.Value - DateTimeOffset.UtcNow).TotalDays <= (ms.WarningThresholdDays ?? MaintenanceSchedule.DefaultWarningThresholdDays))
            )
            .ToListAsync(cancellationToken);
    }
}
