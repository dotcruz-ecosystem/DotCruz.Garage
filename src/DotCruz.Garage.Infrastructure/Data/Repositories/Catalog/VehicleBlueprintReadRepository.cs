using DotCruz.Garage.Application.Abstractions.Data.Repositories.Catalog;
using DotCruz.Garage.Domain.Entities.Catalog;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Catalog;

public class VehicleBlueprintReadRepository(GarageDbContext context)
    : BaseReadRepository<VehicleBlueprint>(context), IVehicleBlueprintReadRepository
{
    public async Task<VehicleBlueprint?> GetByDetailsWithTemplatesAsync(
        string brand,
        string model,
        int year,
        string? engine,
        CancellationToken cancellationToken)
    {
        var normalizedBrand = brand.Trim().ToLower();
        var normalizedModel = model.Trim().ToLower();
        var normalizedEngine = engine?.Trim().ToLower();

        return await _dbSet
            .AsNoTracking()
            .Include(b => b.PlanTemplates)
            .FirstOrDefaultAsync(b =>
                b.Brand.ToLower() == normalizedBrand &&
                b.Model.ToLower() == normalizedModel &&
                b.Year == year &&
                (normalizedEngine == null || b.Engine == null || b.Engine.ToLower() == normalizedEngine),
                cancellationToken);
    }

    public async Task<VehicleBlueprint?> GetByIdWithTemplatesAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(b => b.PlanTemplates)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyCollection<VehicleBlueprint> Items, int TotalCount)> SearchAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm,
        string? brand,
        string? model,
        int? year,
        CancellationToken cancellationToken)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(b => b.PlanTemplates)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(b => b.Brand.ToLower().Contains(term) ||
                                     b.Model.ToLower().Contains(term) ||
                                     (b.Engine != null && b.Engine.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(brand))
        {
            var bTerm = brand.Trim().ToLower();
            query = query.Where(b => b.Brand.ToLower().Contains(bTerm));
        }

        if (!string.IsNullOrWhiteSpace(model))
        {
            var mTerm = model.Trim().ToLower();
            query = query.Where(b => b.Model.ToLower().Contains(mTerm));
        }

        if (year.HasValue)
        {
            query = query.Where(b => b.Year == year.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(b => b.Brand)
            .ThenBy(b => b.Model)
            .ThenBy(b => b.Year)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
