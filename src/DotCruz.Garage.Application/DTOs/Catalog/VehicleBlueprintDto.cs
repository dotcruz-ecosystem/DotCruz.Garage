namespace DotCruz.Garage.Application.DTOs.Catalog;

public record VehicleBlueprintDto(
    Guid Id,
    string Brand,
    string Model,
    int Year,
    string? Engine,
    string? Version,
    string Source,
    IReadOnlyCollection<MaintenancePlanTemplateDto> PlanTemplates);
