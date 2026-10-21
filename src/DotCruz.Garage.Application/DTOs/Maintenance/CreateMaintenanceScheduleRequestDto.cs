using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.DTOs.Maintenance;

public sealed record CreateMaintenanceScheduleRequestDto(
    string Description,
    MaintenanceCategory Category,
    int? IntervalOdometer,
    int? IntervalTimeMonths,
    int? LastPerformedOdometer,
    DateTimeOffset? LastPerformedDate,
    int? WarningThresholdOdometer,
    int? WarningThresholdDays,
    bool RecordInitialLog = false,
    decimal? InitialCost = null,
    string? InitialReceiptUrl = null,
    string? InitialNotes = null
);
