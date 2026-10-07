using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Vehicles;

public class VehicleSaleWriteRepository(GarageDbContext context)
    : BaseWriteRepository<VehicleSale>(context), IVehicleSaleWriteRepository
{
    public async Task<VehicleSale?> GetActiveSaleByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Where(s => s.VehicleId == vehicleId && s.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<VehicleSale?> GetSaleToUpdateUnscopedAsync(Guid saleId, Guid workspaceId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .Where(s => s.Id == saleId && s.WorkspaceId == workspaceId && s.DeletedAt == null)
            .Include(s => s.Vehicle)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
