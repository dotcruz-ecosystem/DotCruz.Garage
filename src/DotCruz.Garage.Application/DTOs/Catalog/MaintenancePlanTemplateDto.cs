namespace DotCruz.Garage.Application.DTOs.Catalog;

public record MaintenancePlanTemplateDto(
    Guid Id,
    string Description,
    string Category,
    int? IntervalOdometer,
    int? IntervalTimeMonths,
    IReadOnlyList<string> Items,
    bool IsRecurring);
