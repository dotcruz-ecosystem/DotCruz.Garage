using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.RevokeVehicleShare;

public class RevokeVehicleShareCommandValidator : AbstractValidator<RevokeVehicleShareCommand>
{
    public RevokeVehicleShareCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.TargetWorkspaceId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);
    }
}
