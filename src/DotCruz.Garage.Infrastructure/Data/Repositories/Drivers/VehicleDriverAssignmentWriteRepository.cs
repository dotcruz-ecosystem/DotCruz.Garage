using DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;
using DotCruz.Garage.Domain.Entities.Drivers;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Drivers;

public class VehicleDriverAssignmentWriteRepository(GarageDbContext context)
    : BaseWriteRepository<VehicleDriverAssignment>(context), IVehicleDriverAssignmentWriteRepository
{
    public async Task<VehicleDriverAssignment?> GetCurrentByVehicleIdToUpdateAsync(Guid vehicleId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.VehicleId == vehicleId && a.EndedAt == null, cancellationToken);
    }
}
