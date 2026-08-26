using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Fines.Commands.CreateTrafficFine;

public class CreateTrafficFineCommandValidator : AbstractValidator<CreateTrafficFineCommand>
{
    public CreateTrafficFineCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.RequestDto.Description)
            .NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY)
            .MaximumLength(500);

        RuleFor(x => x.RequestDto.Amount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.RequestDto.DiscountAmount)
            .GreaterThanOrEqualTo(0).When(x => x.RequestDto.DiscountAmount.HasValue);

        RuleFor(x => x.RequestDto.DueDate)
            .GreaterThanOrEqualTo(x => x.RequestDto.InfractionDate);

        RuleFor(x => x.RequestDto.Status)
            .IsInEnum();

        RuleFor(x => x.RequestDto.WarningThresholdDays)
            .GreaterThan(0).When(x => x.RequestDto.WarningThresholdDays.HasValue);

        RuleFor(x => x.RequestDto.Points)
            .GreaterThanOrEqualTo(0);
    }
}
