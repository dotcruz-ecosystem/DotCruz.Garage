using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.DTOs.Ai;

public record ExtractedMaintenanceItemDto(
    string Description,
    MaintenanceCategory Category,
    int? IntervalOdometer,
    int? IntervalTimeMonths,
    bool IsRequired = true
);
