using DotCruz.Garage.Application.DTOs.Drivers;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Drivers.Commands.RegisterDriver;

public record RegisterDriverCommand(RegisterDriverRequestDto RequestDto) : IRequest<Guid>;
