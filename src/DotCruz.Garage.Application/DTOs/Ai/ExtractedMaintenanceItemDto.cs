using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.DTOs.Ai;

public record ExtractedCheckpointDto(
    int Odometer,
    int Months
);

public record ExtractedMaintenanceItemDto(
    string Description,
    MaintenanceCategory Category,
    int? IntervalOdometer,
    int? IntervalTimeMonths,
    bool IsRequired = true,
    IReadOnlyCollection<ExtractedCheckpointDto>? Checkpoints = null
);
