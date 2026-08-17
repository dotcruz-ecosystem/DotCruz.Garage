namespace DotCruz.Garage.Application.DTOs.Maintenance;

public sealed record SearchMaintenanceSchedulesResponseDto(
    IReadOnlyCollection<MaintenanceScheduleDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount
);
