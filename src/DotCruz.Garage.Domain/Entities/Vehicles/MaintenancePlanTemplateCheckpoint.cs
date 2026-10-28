using DotCruz.Garage.Domain.Entities.Base;

namespace DotCruz.Garage.Domain.Entities.Vehicles;

public class MaintenancePlanTemplateCheckpoint : BaseEntity
{
    public Guid TemplateId { get; private set; }
    public int Odometer { get; private set; }
    public int Months { get; private set; }
    public int Order { get; private set; }

    public virtual MaintenancePlanTemplate Template { get; private init; } = default!;

    private MaintenancePlanTemplateCheckpoint() { }

    public static MaintenancePlanTemplateCheckpoint Create(
        Guid templateId,
        int odometer,
        int months,
        int order)
    {
        return new MaintenancePlanTemplateCheckpoint
        {
            TemplateId = templateId,
            Odometer = odometer,
            Months = months,
            Order = order
        };
    }
}
