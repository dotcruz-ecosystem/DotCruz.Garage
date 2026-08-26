using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.DTOs.Catalog;

public record ExtractedMaintenancePlanItemDto(
    string Description,
    MaintenanceCategory Category,
    int? IntervalOdometer,
    int? IntervalTimeMonths,
    List<string> Items,
    bool IsRecurring = true);

public record ExtractedManualResultDto(
    string Brand,
    string Model,
    int Year,
    string? Engine,
    string? Version,
    List<ExtractedMaintenancePlanItemDto> Templates);
