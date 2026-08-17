using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.CreateVehicle;

public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY)
            .Length(2, 100).WithMessage(ResourceMessagesException.NAME_INVALID_LENGTH);

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage(ResourceMessagesException.BRAND_EMPTY)
            .MaximumLength(50);

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage(ResourceMessagesException.MODEL_EMPTY)
            .MaximumLength(100);

        RuleFor(x => x.Year)
            .InclusiveBetween(1900, DateTime.UtcNow.Year + 1).WithMessage(ResourceMessagesException.YEAR_INVALID);

        RuleFor(x => x.InitialOdometer)
            .GreaterThanOrEqualTo(0).WithMessage(ResourceMessagesException.ODOMETER_NEGATIVE);

        RuleFor(x => x.InitialDailyMileageAverage)
            .GreaterThanOrEqualTo(0).WithMessage(ResourceMessagesException.DAILY_MILEAGE_NEGATIVE);
    }
}
