using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Drivers.Commands.DeleteDriver;

public class DeleteDriverCommandValidator : AbstractValidator<DeleteDriverCommand>
{
    public DeleteDriverCommandValidator()
    {
        RuleFor(x => x.DriverId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);
    }
}
