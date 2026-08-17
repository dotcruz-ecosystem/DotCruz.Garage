using DotCruz.Garage.Application.DTOs.Accessories;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Accessories.Commands.CreateVehicleAccessory;

public sealed record CreateVehicleAccessoryCommand(
    Guid VehicleId,
    CreateVehicleAccessoryRequestDto RequestDto
) : IRequest<Guid>;
