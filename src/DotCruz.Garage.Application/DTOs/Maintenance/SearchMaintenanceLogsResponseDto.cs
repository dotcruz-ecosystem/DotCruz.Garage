namespace DotCruz.Garage.Application.DTOs.Maintenance;

public sealed record SearchMaintenanceLogsResponseDto(
    IReadOnlyCollection<MaintenanceLogDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount
);
