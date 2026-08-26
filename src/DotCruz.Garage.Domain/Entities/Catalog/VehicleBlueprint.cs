using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Domain.Entities.Catalog;

public class VehicleBlueprint : BaseEntity
{
    public string Brand { get; private set; } = null!;
    public string Model { get; private set; } = null!;
    public int Year { get; private set; }
    public string? Engine { get; private set; }
    public string? Version { get; private set; }
    public BlueprintSource Source { get; private set; }

    public virtual ICollection<MaintenancePlanTemplate> PlanTemplates { get; private init; } = new List<MaintenancePlanTemplate>();

    private VehicleBlueprint() { }

    public static VehicleBlueprint Create(
        string brand,
        string model,
        int year,
        string? engine,
        string? version,
        BlueprintSource source = BlueprintSource.AiExtraction)
    {
        return new VehicleBlueprint
        {
            Brand = brand.Trim(),
            Model = model.Trim(),
            Year = year,
            Engine = engine?.Trim(),
            Version = version?.Trim(),
            Source = source
        };
    }

    public void AddPlanTemplate(MaintenancePlanTemplate template)
    {
        PlanTemplates.Add(template);
    }
}
