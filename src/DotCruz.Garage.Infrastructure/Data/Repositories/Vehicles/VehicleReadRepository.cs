using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Vehicles;

public class VehicleReadRepository(GarageDbContext context) 
    : BaseReadRepository<Vehicle>(context), IVehicleReadRepository
{
    public async Task<Vehicle?> GetByIdWithHistoriesAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(v => v.OdometerHistories)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyCollection<Vehicle> Items, int TotalCount)> SearchAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm,
        CancellationToken cancellationToken)
    {
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(v => v.Name.Contains(searchTerm) || 
                                     v.Brand.Contains(searchTerm) || 
                                     v.Model.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(v => v.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyCollection<Vehicle>> GetVehiclesByAttributesAsync(
        string brand,
        string model,
        string engine,
        int? yearFrom,
        int? yearTo,
        CancellationToken cancellationToken)
    {
        var brandLower = brand.ToLower();
        var modelLower = model.ToLower();
        var engineLower = engine.ToLower();

        var query = _dbSet.AsNoTracking()
            .IgnoreQueryFilters()
            .Where(v => v.DeletedAt == null &&
                        v.Brand.ToLower() == brandLower &&
                        v.Model.ToLower() == modelLower &&
                        v.Engine.ToLower() == engineLower);

        if (yearFrom.HasValue && yearFrom.Value > 0)
        {
            query = query.Where(v => v.Year >= yearFrom.Value);
        }

        if (yearTo.HasValue && yearTo.Value > 0)
        {
            query = query.Where(v => v.Year <= yearTo.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
