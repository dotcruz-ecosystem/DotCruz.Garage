using DotCruz.Garage.Application.DTOs.Drivers;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Drivers.Commands.UpdateDriver;

public record UpdateDriverCommand(Guid DriverId, UpdateDriverRequestDto RequestDto) : IRequest;
