namespace DotCruz.Garage.Application.DTOs.Vehicles;

public sealed record ClaimPostSaleVehicleResponseDto(
    Guid VehicleId,
    string VehicleName,
    string Brand,
    string Model,
    int Year,
    string? Plate,
    DateTimeOffset ClaimedAt
);
