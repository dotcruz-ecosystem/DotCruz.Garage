using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Domain.Entities.Catalog;

public class MaintenancePlanTemplate : BaseEntity
{
    public Guid VehicleBlueprintId { get; private set; }
    public string Description { get; private set; } = null!;
    public MaintenanceCategory Category { get; private set; }
    public int? IntervalOdometer { get; private set; }
    public int? IntervalTimeMonths { get; private set; }
    public List<string> Items { get; private set; } = new();
    public bool IsRecurring { get; private set; }

    public virtual VehicleBlueprint VehicleBlueprint { get; private init; } = default!;

    private MaintenancePlanTemplate() { }

    public static MaintenancePlanTemplate Create(
        Guid vehicleBlueprintId,
        string description,
        MaintenanceCategory category,
        int? intervalOdometer,
        int? intervalTimeMonths,
        List<string>? items = null,
        bool isRecurring = true)
    {
        return new MaintenancePlanTemplate
        {
            VehicleBlueprintId = vehicleBlueprintId,
            Description = description.Trim(),
            Category = category,
            IntervalOdometer = intervalOdometer,
            IntervalTimeMonths = intervalTimeMonths,
            Items = items ?? new List<string>(),
            IsRecurring = isRecurring
        };
    }
}
