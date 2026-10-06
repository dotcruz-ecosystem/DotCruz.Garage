namespace DotCruz.Garage.Application.DTOs.Vehicles;

public sealed record VehicleSaleDto(
    Guid Id,
    Guid VehicleId,
    string VehicleName,
    string CustomerName,
    string CustomerEmail,
    string? CustomerPhone,
    string? CustomerDocument,
    DateTimeOffset SoldAt,
    decimal? SoldPrice,
    int SoldOdometer,
    DateTimeOffset? WarrantyExpiresAt,
    int? WarrantyKm,
    bool IsWarrantyActive,
    int? RemainingWarrantyDays,
    int? RemainingWarrantyKm,
    string? Notes,
    Guid? ClaimedByUserId,
    DateTimeOffset? ClaimedAt,
    bool IsActive
);
