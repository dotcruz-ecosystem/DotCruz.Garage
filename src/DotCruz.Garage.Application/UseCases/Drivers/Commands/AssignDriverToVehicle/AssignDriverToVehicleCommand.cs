using DotCruz.Garage.Application.DTOs.Drivers;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Drivers.Commands.AssignDriverToVehicle;

public record AssignDriverToVehicleCommand(Guid DriverId, AssignDriverRequestDto RequestDto) : IRequest<Guid>;
