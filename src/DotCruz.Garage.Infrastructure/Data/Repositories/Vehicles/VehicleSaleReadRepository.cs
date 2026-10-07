using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Vehicles;

public class VehicleSaleReadRepository(GarageDbContext context)
    : BaseReadRepository<VehicleSale>(context), IVehicleSaleReadRepository
{
    public async Task<IReadOnlyCollection<VehicleSale>> GetActiveSalesWithVehicleAsync(CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(s => s.Vehicle)
            .Where(s => s.IsActive)
            .OrderByDescending(s => s.SoldAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<VehicleSale?> GetByIdWithVehicleAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(s => s.Vehicle)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<VehicleSale?> GetByVehicleIdWithVehicleAsync(Guid vehicleId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(s => s.Vehicle)
            .Where(s => s.VehicleId == vehicleId && s.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<VehicleSale?> GetSaleUnscopedAsync(Guid saleId, Guid workspaceId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Include(s => s.Vehicle)
            .Where(s => s.Id == saleId && s.WorkspaceId == workspaceId && s.DeletedAt == null)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
