using DotCruz.Garage.Application.DTOs.Documents;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Documents.Commands.PayVehicleDocument;

public sealed record PayVehicleDocumentCommand(
    Guid DocumentId,
    PayDocumentRequestDto Request
) : IRequest;
