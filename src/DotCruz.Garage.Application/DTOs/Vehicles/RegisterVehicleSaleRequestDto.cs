namespace DotCruz.Garage.Application.DTOs.Vehicles;

public sealed record RegisterVehicleSaleRequestDto(
    string CustomerName,
    string CustomerEmail,
    string? CustomerPhone = null,
    string? CustomerDocument = null,
    DateTimeOffset? SoldAt = null,
    decimal? SoldPrice = null,
    int? SoldOdometer = null,
    DateTimeOffset? WarrantyExpiresAt = null,
    int? WarrantyKm = null,
    string? Notes = null
);
