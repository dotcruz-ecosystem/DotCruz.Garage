using DotCruz.Garage.Application.DTOs.Catalog;
using DotCruz.Garage.Domain.Entities.Catalog;

namespace DotCruz.Garage.Application.Mappers.Catalog;

public static class VehicleBlueprintMapper
{
    public static VehicleBlueprintDto ToDto(this VehicleBlueprint blueprint)
    {
        return new VehicleBlueprintDto(
            blueprint.Id,
            blueprint.Brand,
            blueprint.Model,
            blueprint.Year,
            blueprint.Engine,
            blueprint.Version,
            blueprint.Source.ToString(),
            blueprint.PlanTemplates.Select(t => t.ToDto()).ToList()
        );
    }

    public static MaintenancePlanTemplateDto ToDto(this MaintenancePlanTemplate template)
    {
        return new MaintenancePlanTemplateDto(
            template.Id,
            template.Description,
            template.Category.ToString(),
            template.IntervalOdometer,
            template.IntervalTimeMonths,
            template.Items.AsReadOnly(),
            template.IsRecurring
        );
    }
}
