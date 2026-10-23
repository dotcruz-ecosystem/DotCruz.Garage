using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.DTOs.Blueprints;

public sealed record VehicleBlueprintDto(
    Guid Id,
    string Brand,
    string Model,
    string Engine,
    VehicleType Type,
    int YearFrom,
    int YearTo,
    string? SourceUrl,
    BlueprintStatus Status,
    string? ErrorMessage,
    IReadOnlyCollection<MaintenancePlanTemplateDto> Templates
);

public sealed record MaintenancePlanTemplateDto(
    Guid Id,
    string Description,
    MaintenanceCategory Category,
    int? IntervalOdometer,
    int? IntervalTimeMonths,
    bool IsRequired,
    bool AlreadyScheduled
);
