using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.ProcessOdometerCheckin;

public class ProcessOdometerCheckinCommandValidator : AbstractValidator<ProcessOdometerCheckinCommand>
{
    public ProcessOdometerCheckinCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.TOKEN_REQUIRED);

        RuleFor(x => x.NewOdometer)
            .GreaterThan(0)
            .WithMessage(ResourceMessagesException.ODOMETER_NEGATIVE);
    }
}
