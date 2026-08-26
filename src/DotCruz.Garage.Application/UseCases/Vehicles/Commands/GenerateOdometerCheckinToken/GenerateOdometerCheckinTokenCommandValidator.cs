using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.GenerateOdometerCheckinToken;

public class GenerateOdometerCheckinTokenCommandValidator : AbstractValidator<GenerateOdometerCheckinTokenCommand>
{
    public GenerateOdometerCheckinTokenCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.ID_EMPTY);
    }
}
