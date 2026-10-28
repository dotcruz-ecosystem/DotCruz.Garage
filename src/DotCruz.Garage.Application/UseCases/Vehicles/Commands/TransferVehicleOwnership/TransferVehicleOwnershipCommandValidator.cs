using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.TransferVehicleOwnership;

public class TransferVehicleOwnershipCommandValidator : AbstractValidator<TransferVehicleOwnershipCommand>
{
    public TransferVehicleOwnershipCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.RequestDto.NewOwnerWorkspaceId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);
    }
}
