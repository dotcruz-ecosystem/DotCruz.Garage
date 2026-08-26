using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Catalog.Commands.ApplyBlueprintToVehicle;

public class ApplyBlueprintToVehicleCommandValidator : AbstractValidator<ApplyBlueprintToVehicleCommand>
{
    public ApplyBlueprintToVehicleCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.BlueprintId)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.ID_EMPTY);
    }
}
