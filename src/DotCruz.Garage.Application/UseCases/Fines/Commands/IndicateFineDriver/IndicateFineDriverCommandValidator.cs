using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Fines.Commands.IndicateFineDriver;

public class IndicateFineDriverCommandValidator : AbstractValidator<IndicateFineDriverCommand>
{
    public IndicateFineDriverCommandValidator()
    {
        RuleFor(x => x.FineId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.RequestDto.DriverId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);
    }
}
