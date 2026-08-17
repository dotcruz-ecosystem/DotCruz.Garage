using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Documents.Commands.PayVehicleDocument;

public class PayVehicleDocumentCommandValidator : AbstractValidator<PayVehicleDocumentCommand>
{
    public PayVehicleDocumentCommandValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.Request.FileUrl)
            .NotEmpty().WithMessage(ResourceMessagesException.DOCUMENT_FILE_REQUIRED);
    }
}
