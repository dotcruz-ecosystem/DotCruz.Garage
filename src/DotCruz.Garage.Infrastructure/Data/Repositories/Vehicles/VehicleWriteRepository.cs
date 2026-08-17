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
}
