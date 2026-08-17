using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Accessories.Commands.CreateVehicleAccessory;

public class CreateVehicleAccessoryCommandValidator : AbstractValidator<CreateVehicleAccessoryCommand>
{
    public CreateVehicleAccessoryCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.RequestDto.Name)
            .NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY)
            .MaximumLength(200);

        RuleFor(x => x.RequestDto.Cost)
            .GreaterThanOrEqualTo(0);
    }   
}
