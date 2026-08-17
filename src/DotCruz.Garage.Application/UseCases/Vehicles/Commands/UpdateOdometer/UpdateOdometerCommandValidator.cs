using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.UpdateOdometer;

public class UpdateOdometerCommandValidator : AbstractValidator<UpdateOdometerCommand>
{
    public UpdateOdometerCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.RequestDto.NewOdometer)
            .GreaterThanOrEqualTo(0).WithMessage(ResourceMessagesException.ODOMETER_NEGATIVE);
    }
}
