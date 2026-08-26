using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Drivers.Commands.AssignDriverToVehicle;

public class AssignDriverToVehicleCommandValidator : AbstractValidator<AssignDriverToVehicleCommand>
{
    public AssignDriverToVehicleCommandValidator()
    {
        RuleFor(x => x.DriverId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.RequestDto.VehicleId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);
    }
}
