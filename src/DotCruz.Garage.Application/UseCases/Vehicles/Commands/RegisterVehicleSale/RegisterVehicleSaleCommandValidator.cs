using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.RegisterVehicleSale;

public class RegisterVehicleSaleCommandValidator : AbstractValidator<RegisterVehicleSaleCommand>
{
    public RegisterVehicleSaleCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage("VehicleId não pode ser vazio.");

        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Nome do cliente não pode ser vazio.")
            .MaximumLength(150).WithMessage("Nome do cliente deve ter no máximo 150 caracteres.");

        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("E-mail do cliente não pode ser vazio.")
            .EmailAddress().WithMessage("E-mail do cliente inválido.")
            .MaximumLength(150).WithMessage("E-mail do cliente deve ter no máximo 150 caracteres.");

        RuleFor(x => x.SoldOdometer)
            .GreaterThanOrEqualTo(0).When(x => x.SoldOdometer.HasValue)
            .WithMessage("Quilometragem de venda não pode ser negativa.");

        RuleFor(x => x.WarrantyKm)
            .GreaterThan(0).When(x => x.WarrantyKm.HasValue)
            .WithMessage("Quilometragem de garantia deve ser maior que zero.");

        RuleFor(x => x.SoldPrice)
            .GreaterThan(0).When(x => x.SoldPrice.HasValue)
            .WithMessage("Preço de venda deve ser maior que zero.");
    }
}
