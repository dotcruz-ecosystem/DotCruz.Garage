using DotCruz.Garage.Application.DTOs.Fines;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Fines.Commands.PayTrafficFine;

public sealed record PayTrafficFineCommand(
    Guid FineId,
    PayFineRequestDto RequestDto
) : IRequest;
