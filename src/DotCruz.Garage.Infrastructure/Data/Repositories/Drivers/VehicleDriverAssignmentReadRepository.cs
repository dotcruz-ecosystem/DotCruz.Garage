using DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;
using DotCruz.Garage.Domain.Entities.Drivers;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Drivers;

public class VehicleDriverAssignmentReadRepository(GarageDbContext context)
    : BaseReadRepository<VehicleDriverAssignment>(context), IVehicleDriverAssignmentReadRepository
{
    public async Task<IReadOnlyCollection<VehicleDriverAssignment>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken)
    {
        return await _dbSet.AsNoTracking()
            .Include(a => a.Driver)
            .Where(a => a.VehicleId == vehicleId)
            .OrderByDescending(a => a.StartedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<VehicleDriverAssignment>> GetByDriverIdAsync(Guid driverId, CancellationToken cancellationToken)
    {
        return await _dbSet.AsNoTracking()
            .Include(a => a.Vehicle)
            .Where(a => a.DriverId == driverId)
            .OrderByDescending(a => a.StartedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<VehicleDriverAssignment?> GetCurrentByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken)
    {
        return await _dbSet.AsNoTracking()
            .Include(a => a.Driver)
            .FirstOrDefaultAsync(a => a.VehicleId == vehicleId && a.EndedAt == null, cancellationToken);
    }
}
