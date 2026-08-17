namespace DotCruz.Garage.Application.DTOs.Accessories;

public record VehicleAccessoryDto(
    Guid Id,
    Guid VehicleId,
    string Name,
    string? Store,
    DateTimeOffset PurchaseDate,
    decimal Cost,
    string? ReceiptUrl,
    string? Notes
);
