using DotCruz.Garage.Application.DTOs.Vehicles;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.ClaimPostSaleVehicle;

public sealed record ClaimPostSaleVehicleCommand(string Token) : IRequest<ClaimPostSaleVehicleResponseDto>;
