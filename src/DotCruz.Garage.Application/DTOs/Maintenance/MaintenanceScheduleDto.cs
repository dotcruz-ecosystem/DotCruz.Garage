using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.DTOs.Maintenance;

public record MaintenanceScheduleCheckpointDto(
    Guid Id,
    int Odometer,
    int Months,
    int Order,
    bool IsCompleted,
    DateTimeOffset? CompletedAt,
    int? CompletedOdometer
);

public record MaintenanceScheduleDto(
    Guid Id,
    Guid VehicleId,
    string Description,
    MaintenanceCategory Category,
    int? IntervalOdometer,
    int? IntervalTimeMonths,
    int? LastPerformedOdometer,
    DateTimeOffset? LastPerformedDate,
    int? NextDueOdometer,
    DateTimeOffset? NextDueDate,
    string Status,
    int? WarningThresholdOdometer,
    int? WarningThresholdDays,
    IReadOnlyCollection<MaintenanceScheduleCheckpointDto>? Checkpoints = null
);
