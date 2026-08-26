using DotCruz.Garage.Application.DTOs.Drivers;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Drivers.Queries.GetDriverById;

public record GetDriverByIdQuery(Guid DriverId) : IRequest<DriverDto>;
