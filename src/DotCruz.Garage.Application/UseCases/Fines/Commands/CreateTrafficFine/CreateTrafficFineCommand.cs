using DotCruz.Garage.Application.DTOs.Fines;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Fines.Commands.CreateTrafficFine;

public sealed record CreateTrafficFineCommand(
    Guid VehicleId,
    CreateTrafficFineRequestDto RequestDto
) : IRequest<Guid>;
