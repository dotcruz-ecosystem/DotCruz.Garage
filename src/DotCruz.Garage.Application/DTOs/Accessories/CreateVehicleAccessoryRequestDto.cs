namespace DotCruz.Garage.Application.DTOs.Accessories;

public sealed record CreateVehicleAccessoryRequestDto(
    string Name,
    string? Store,
    DateTimeOffset PurchaseDate,
    decimal Cost,
    string? ReceiptUrl,
    string? Notes
);
