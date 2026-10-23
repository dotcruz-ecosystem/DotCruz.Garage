using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Vehicles;

public class VehicleBlueprintRepository : IVehicleBlueprintRepository
{
    private readonly GarageDbContext _context;

    public VehicleBlueprintRepository(GarageDbContext context)
    {
        _context = context;
    }

    public async Task<VehicleBlueprint?> FindMatchingBlueprintAsync(
        string brand,
        string model,
        string engine,
        int year,
        VehicleType type,
        CancellationToken cancellationToken)
    {
        var normBrand = brand.Trim().ToLower();
        var normModel = model.Trim().ToLower();
        var normEngine = engine.Trim().ToLower();

        return await _context.VehicleBlueprints
            .Include(b => b.Templates)
            .Where(b => b.Type == type &&
                        b.Brand.ToLower() == normBrand &&
                        b.Model.ToLower() == normModel &&
                        b.Engine.ToLower() == normEngine &&
                        b.YearFrom <= year &&
                        b.YearTo >= year)
            .OrderByDescending(b => b.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<VehicleBlueprint?> GetByIdWithTemplatesAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.VehicleBlueprints
            .Include(b => b.Templates)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<VehicleBlueprint>> GetPendingBlueprintsAsync(int limit, CancellationToken cancellationToken)
    {
        return await _context.VehicleBlueprints
            .Where(b => b.Status == BlueprintStatus.Pending)
            .OrderBy(b => b.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(VehicleBlueprint blueprint, CancellationToken cancellationToken)
    {
        await _context.VehicleBlueprints.AddAsync(blueprint, cancellationToken);
    }
}
