using DotCruz.Garage.Application.Abstractions.Data.Repositories.Accessories;
using DotCruz.Garage.Domain.Entities.Accessories;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Accessories;

public class VehicleAccessoryReadRepository(GarageDbContext context)
    : BaseReadRepository<VehicleAccessory>(context), IVehicleAccessoryReadRepository
{
    public async Task<IReadOnlyCollection<VehicleAccessory>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken)
    {
        return await _dbSet.AsNoTracking()
            .Where(a => a.VehicleId == vehicleId)
            .OrderByDescending(a => a.PurchaseDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyCollection<VehicleAccessory> Items, int TotalCount)> SearchAsync(
        int pageNumber,
        int pageSize,
        Guid? vehicleId,
        CancellationToken cancellationToken)
    {
        var query = _dbSet.AsNoTracking();

        if (vehicleId.HasValue)
        {
            query = query.Where(a => a.VehicleId == vehicleId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.PurchaseDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
