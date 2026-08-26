using DotCruz.Garage.Application.Abstractions.Data.Repositories.Fines;
using DotCruz.Garage.Domain.Entities.Fines;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Fines;

public class TrafficFineReadRepository(GarageDbContext context)
    : BaseReadRepository<TrafficFine>(context), ITrafficFineReadRepository
{
    public async Task<IReadOnlyCollection<TrafficFine>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken)
    {
        return await _dbSet.AsNoTracking()
            .Where(f => f.VehicleId == vehicleId)
            .OrderByDescending(f => f.InfractionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<TrafficFine>> GetByDriverIdAsync(Guid driverId, CancellationToken cancellationToken)
    {
        return await _dbSet.AsNoTracking()
            .Where(f => f.DriverId == driverId)
            .OrderByDescending(f => f.InfractionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<TrafficFine>> GetPendingNICFinesAsync(CancellationToken cancellationToken)
    {
        return await _dbSet.AsNoTracking()
            .Include(f => f.Vehicle)
            .Where(f => f.Status != Domain.Enums.FineStatus.Paid && f.Status != Domain.Enums.FineStatus.Indicated && f.IndicatedAt == null && f.IndicationDeadline != null)
            .OrderBy(f => f.IndicationDeadline)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyCollection<TrafficFine> Items, int TotalCount)> SearchAsync(
        int pageNumber,
        int pageSize,
        Guid? vehicleId,
        CancellationToken cancellationToken)
    {
        var query = _dbSet.AsNoTracking();

        if (vehicleId.HasValue)
        {
            query = query.Where(f => f.VehicleId == vehicleId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(f => f.InfractionDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
