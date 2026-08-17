using DotCruz.Garage.Application.DTOs.Vehicles;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.UpdateOdometer;

public sealed record UpdateOdometerCommand(
    Guid VehicleId,
    UpdateOdometerRequestDto RequestDto
) : IRequest;
