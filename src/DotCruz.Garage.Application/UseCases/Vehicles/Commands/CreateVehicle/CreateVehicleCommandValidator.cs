using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.CreateVehicle;

public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.Name), () =>
        {
            RuleFor(x => x.Name!)
                .Length(2, 100).WithMessage(ResourceMessagesException.NAME_INVALID_LENGTH);
        });

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage(ResourceMessagesException.VEHICLE_TYPE_INVALID);

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage(ResourceMessagesException.BRAND_EMPTY)
            .MaximumLength(50);

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage(ResourceMessagesException.MODEL_EMPTY)
            .MaximumLength(100);

        RuleFor(x => x.Engine)
            .NotEmpty().WithMessage(ResourceMessagesException.ENGINE_EMPTY)
            .MaximumLength(50);

        RuleFor(x => x.Year)
            .InclusiveBetween(1900, DateTime.UtcNow.Year + 1).WithMessage(ResourceMessagesException.YEAR_INVALID);

        When(x => !string.IsNullOrWhiteSpace(x.Plate), () =>
        {
            RuleFor(x => x.Plate!)
                .MaximumLength(10);
        });

        RuleFor(x => x.InitialOdometer)
            .GreaterThanOrEqualTo(0).WithMessage(ResourceMessagesException.ODOMETER_NEGATIVE);

        RuleFor(x => x.InitialDailyMileageAverage)
            .GreaterThanOrEqualTo(0).WithMessage(ResourceMessagesException.DAILY_MILEAGE_NEGATIVE);
    }
}
