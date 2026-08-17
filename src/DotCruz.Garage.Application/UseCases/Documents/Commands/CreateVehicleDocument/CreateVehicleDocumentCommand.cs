using DotCruz.Garage.Application.DTOs.Documents;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Documents.Commands.CreateVehicleDocument;

public sealed record CreateVehicleDocumentCommand(
    Guid VehicleId,
    CreateVehicleDocumentRequestDto RequestDto
) : IRequest<Guid>;
