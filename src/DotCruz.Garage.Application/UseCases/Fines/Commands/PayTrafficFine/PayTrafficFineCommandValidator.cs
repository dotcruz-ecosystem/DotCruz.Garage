using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Fines.Commands.PayTrafficFine;

public class PayTrafficFineCommandValidator : AbstractValidator<PayTrafficFineCommand>
{
    public PayTrafficFineCommandValidator()
    {
        RuleFor(x => x.FineId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.RequestDto.ReceiptUrl)
            .NotEmpty().WithMessage(ResourceMessagesException.PAYMENT_RECEIPT_REQUIRED);
    }
}
