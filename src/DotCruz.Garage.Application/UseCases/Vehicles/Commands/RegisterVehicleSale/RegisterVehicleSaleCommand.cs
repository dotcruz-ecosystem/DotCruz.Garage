using DotCruz.Garage.Application.DTOs.Vehicles;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.RegisterVehicleSale;

public record RegisterVehicleSaleCommand(
    Guid VehicleId,
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
) : IRequest<RegisterVehicleSaleResponseDto>;
