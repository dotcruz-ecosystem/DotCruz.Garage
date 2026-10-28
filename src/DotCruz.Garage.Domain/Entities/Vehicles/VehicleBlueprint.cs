using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Domain.Entities.Vehicles;

public class VehicleBlueprint : BaseEntity
{
    public string Brand { get; private set; } = null!;
    public string Model { get; private set; } = null!;
    public string Engine { get; private set; } = null!;
    public VehicleType Type { get; private set; }
    public int YearFrom { get; private set; }
    public int YearTo { get; private set; }
    public string? SourceUrl { get; private set; }
    public BlueprintStatus Status { get; private set; }
    public string? ErrorMessage { get; private set; }

    public virtual ICollection<MaintenancePlanTemplate> Templates { get; private init; } = new List<MaintenancePlanTemplate>();

    private VehicleBlueprint() { }

    public static VehicleBlueprint Create(
        string brand,
        string model,
        string engine,
        VehicleType type,
        int yearFrom,
        int yearTo,
        string? sourceUrl = null)
    {
        return new VehicleBlueprint
        {
            Brand = brand.Trim(),
            Model = model.Trim(),
            Engine = engine.Trim(),
            Type = type,
            YearFrom = yearFrom,
            YearTo = yearTo,
            SourceUrl = sourceUrl,
            Status = BlueprintStatus.Pending
        };
    }

    public void MarkAsProcessing()
    {
        Status = BlueprintStatus.Processing;
        ErrorMessage = null;
        Touch();
    }

    public void MarkAsReady(string? sourceUrl, int yearFrom, int yearTo)
    {
        Status = BlueprintStatus.Ready;
        SourceUrl = sourceUrl ?? SourceUrl;
        YearFrom = yearFrom;
        YearTo = yearTo;
        ErrorMessage = null;
        Touch();
    }

    public void MarkAsFailed(string errorMessage)
    {
        Status = BlueprintStatus.Failed;
        ErrorMessage = errorMessage;
        Touch();
    }

    public MaintenancePlanTemplate AddTemplate(
        string description,
        MaintenanceCategory category,
        int? intervalOdometer,
        int? intervalTimeMonths,
        bool isRequired = true,
        IEnumerable<(int Odometer, int Months)>? checkpoints = null)
    {
        var template = MaintenancePlanTemplate.Create(Id, description, category, intervalOdometer, intervalTimeMonths, isRequired);
        if (checkpoints != null)
        {
            int order = 1;
            foreach (var cp in checkpoints.OrderBy(c => c.Odometer))
            {
                template.AddCheckpoint(cp.Odometer, cp.Months, order++);
            }
        }
        Templates.Add(template);
        return template;
    }
}
