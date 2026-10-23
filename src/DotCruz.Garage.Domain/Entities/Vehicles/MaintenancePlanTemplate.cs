using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Domain.Entities.Vehicles;

public class MaintenancePlanTemplate : BaseEntity
{
    public Guid BlueprintId { get; private set; }
    public string Description { get; private set; } = null!;
    public MaintenanceCategory Category { get; private set; }
    public int? IntervalOdometer { get; private set; }
    public int? IntervalTimeMonths { get; private set; }
    public bool IsRequired { get; private set; }

    public virtual VehicleBlueprint Blueprint { get; private init; } = default!;

    private MaintenancePlanTemplate() { }

    public static MaintenancePlanTemplate Create(
        Guid blueprintId,
        string description,
        MaintenanceCategory category,
        int? intervalOdometer,
        int? intervalTimeMonths,
        bool isRequired = true)
    {
        return new MaintenancePlanTemplate
        {
            BlueprintId = blueprintId,
            Description = description.Trim(),
            Category = category,
            IntervalOdometer = intervalOdometer,
            IntervalTimeMonths = intervalTimeMonths,
            IsRequired = isRequired
        };
    }
}
