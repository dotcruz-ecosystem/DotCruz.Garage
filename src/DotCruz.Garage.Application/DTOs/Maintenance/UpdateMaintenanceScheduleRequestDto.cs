using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.DTOs.Maintenance;

public record UpdateMaintenanceScheduleRequestDto(
    string Description,
    MaintenanceCategory Category,
    int? IntervalOdometer = null,
    int? IntervalTimeMonths = null,
    int? WarningThresholdOdometer = null,
    int? WarningThresholdDays = null);
