namespace DotCruz.Garage.Application.DTOs.Maintenance;

public record MaintenanceOverviewDto(
    Guid VehicleId,
    int TotalSchedules,
    int ActiveSchedules,
    int DueSoonSchedules,
    int OverdueSchedules,
    int InactiveSchedules,
    decimal TotalMaintenanceCostLast12Months,
    int TotalServicesPerformed);
