using MediatR;

namespace DotCruz.Garage.Application.UseCases.Drivers.Commands.DeleteDriver;

public record DeleteDriverCommand(Guid DriverId) : IRequest;
