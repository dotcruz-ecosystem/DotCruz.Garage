using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.SubmitPostSaleOdometer;

public class SubmitPostSaleOdometerCommandValidator : AbstractValidator<SubmitPostSaleOdometerCommand>
{
    public SubmitPostSaleOdometerCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token do portal pós-venda não pode ser vazio.");

        RuleFor(x => x.NewOdometer)
            .GreaterThanOrEqualTo(0).WithMessage("Quilometragem não pode ser negativa.");
    }
}
