using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Vehicles;

public class VehicleCatalogRepository : IVehicleCatalogRepository
{
    private readonly GarageDbContext _context;

    public VehicleCatalogRepository(GarageDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<string>> GetBrandsAsync(VehicleType? type, CancellationToken cancellationToken)
    {
        var query = _context.VehicleBrands.AsNoTracking();

        if (type.HasValue)
        {
            query = query.Where(b => b.Models.Any(m => m.Type == type.Value));
        }

        return await query
            .OrderBy(b => b.Name)
            .Select(b => b.Name)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> GetModelsAsync(string brandName, VehicleType? type, CancellationToken cancellationToken)
    {
        var normalizedBrand = brandName.Trim().ToLower();

        var query = _context.VehicleModels
            .AsNoTracking()
            .Where(m => m.Brand.Name.ToLower() == normalizedBrand);

        if (type.HasValue)
        {
            query = query.Where(m => m.Type == type.Value);
        }

        return await query
            .OrderBy(m => m.Name)
            .Select(m => m.Name)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> GetEnginesAsync(VehicleType? type, CancellationToken cancellationToken)
    {
        var query = _context.VehicleEngines.AsNoTracking();

        if (type.HasValue)
        {
            query = query.Where(e => e.Type == null || e.Type == type.Value);
        }

        var engines = await query
            .Select(e => e.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

        return SortEnginesNaturally(engines);
    }

    public static IReadOnlyCollection<string> SortEnginesNaturally(IEnumerable<string> engines)
    {
        return engines
            .Select(e => e.Trim())
            .Distinct()
            .OrderBy(e =>
            {
                if (e.Contains("elétric", StringComparison.OrdinalIgnoreCase) || 
                    e.Contains("híbrid", StringComparison.OrdinalIgnoreCase))
                {
                    return 2;
                }

                var match = System.Text.RegularExpressions.Regex.Match(e, @"^(\d+(?:[.,]\d+)?)");
                return match.Success ? 0 : 1;
            })
            .ThenBy(e =>
            {
                var match = System.Text.RegularExpressions.Regex.Match(e, @"^(\d+(?:[.,]\d+)?)");
                if (match.Success && decimal.TryParse(match.Groups[1].Value.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture, out var num))
                {
                    return num;
                }
                return decimal.MaxValue;
            })
            .ThenBy(e => e)
            .ToList();
    }

    public async Task<VehicleBrand> EnsureBrandExistsAsync(string brandName, CancellationToken cancellationToken)
    {
        var trimmed = brandName.Trim();
        var normalized = trimmed.ToLower();

        var brand = await _context.VehicleBrands
            .FirstOrDefaultAsync(b => b.Name.ToLower() == normalized, cancellationToken);

        if (brand == null)
        {
            brand = VehicleBrand.Create(trimmed);
            await _context.VehicleBrands.AddAsync(brand, cancellationToken);
        }

        return brand;
    }

    public async Task<VehicleModel> EnsureModelExistsAsync(Guid brandId, string modelName, VehicleType type, CancellationToken cancellationToken)
    {
        var trimmed = modelName.Trim();
        var normalized = trimmed.ToLower();

        var model = await _context.VehicleModels
            .FirstOrDefaultAsync(m => m.BrandId == brandId && m.Type == type && m.Name.ToLower() == normalized, cancellationToken);

        if (model == null)
        {
            model = VehicleModel.Create(brandId, trimmed, type);
            await _context.VehicleModels.AddAsync(model, cancellationToken);
        }

        return model;
    }

    public async Task<VehicleEngine> EnsureEngineExistsAsync(string engineName, VehicleType? type, CancellationToken cancellationToken)
    {
        var trimmed = engineName.Trim();
        var normalized = trimmed.ToLower();

        var engine = await _context.VehicleEngines
            .FirstOrDefaultAsync(e => e.Name.ToLower() == normalized && (e.Type == type || e.Type == null), cancellationToken);

        if (engine == null)
        {
            engine = VehicleEngine.Create(trimmed, type);
            await _context.VehicleEngines.AddAsync(engine, cancellationToken);
        }

        return engine;
    }
}
