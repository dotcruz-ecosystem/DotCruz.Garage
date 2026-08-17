using DotCruz.Garage.Application.Abstractions.Data.Repositories.Documents;
using DotCruz.Garage.Domain.Entities.Documents;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Documents;

public class VehicleDocumentReadRepository(GarageDbContext context)
    : BaseReadRepository<VehicleDocument>(context), IVehicleDocumentReadRepository
{
    public async Task<IReadOnlyCollection<VehicleDocument>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken)
    {
        return await _dbSet.AsNoTracking()
            .Where(d => d.VehicleId == vehicleId)
            .OrderByDescending(d => d.ReferenceYear)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyCollection<VehicleDocument> Items, int TotalCount)> SearchAsync(
        int pageNumber,
        int pageSize,
        Guid? vehicleId,
        CancellationToken cancellationToken)
    {
        var query = _dbSet.AsNoTracking();

        if (vehicleId.HasValue)
        {
            query = query.Where(d => d.VehicleId == vehicleId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(d => d.ReferenceYear)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
