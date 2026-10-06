namespace DotCruz.Garage.Application.DTOs.Vehicles;

public sealed record PostSalePortalDto(
    Guid VehicleId,
    string VehicleName,
    string Brand,
    string Model,
    int Year,
    string? Plate,
    int CurrentOdometer,
    string CustomerName,
    DateTimeOffset SoldAt,
    DateTimeOffset? WarrantyExpiresAt,
    int? WarrantyKm,
    bool IsWarrantyActive,
    int? RemainingWarrantyDays,
    int? RemainingWarrantyKm,
    IReadOnlyCollection<PostSaleMaintenanceScheduleDto> UpcomingMaintenance,
    IReadOnlyCollection<PostSaleMaintenanceLogDto> MaintenanceHistory
);

public sealed record PostSaleMaintenanceScheduleDto(
    Guid Id,
    string Description,
    string Category,
    int? NextDueOdometer,
    DateTimeOffset? NextDueDate,
    string Status
);

public sealed record PostSaleMaintenanceLogDto(
    Guid Id,
    string Description,
    DateTimeOffset PerformedDate,
    int PerformedOdometer,
    decimal Cost
);
