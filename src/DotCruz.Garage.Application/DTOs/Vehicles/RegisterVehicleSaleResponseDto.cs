namespace DotCruz.Garage.Application.DTOs.Vehicles;

public sealed record RegisterVehicleSaleResponseDto(
    Guid SaleId,
    Guid VehicleId,
    string CustomerName,
    string CustomerEmail,
    string PortalUrl,
    string PortalToken
);
