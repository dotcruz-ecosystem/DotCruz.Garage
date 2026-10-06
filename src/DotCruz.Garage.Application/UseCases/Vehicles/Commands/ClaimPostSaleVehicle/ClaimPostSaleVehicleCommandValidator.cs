using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.ClaimPostSaleVehicle;

public class ClaimPostSaleVehicleCommandValidator : AbstractValidator<ClaimPostSaleVehicleCommand>
{
    public ClaimPostSaleVehicleCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token do portal pós-venda não pode ser vazio.");
    }
}
