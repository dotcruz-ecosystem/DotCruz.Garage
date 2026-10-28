using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.ShareVehicle;

public class ShareVehicleCommandValidator : AbstractValidator<ShareVehicleCommand>
{
    public ShareVehicleCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.RequestDto.TargetWorkspaceId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.RequestDto.Role)
            .IsInEnum().WithMessage(ResourceMessagesException.UNKNOWN_ERROR);
    }
}
