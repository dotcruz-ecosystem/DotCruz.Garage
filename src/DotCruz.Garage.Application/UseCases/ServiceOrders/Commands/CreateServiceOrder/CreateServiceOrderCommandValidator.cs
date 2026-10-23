using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.ServiceOrders.Commands.CreateServiceOrder;

public class CreateServiceOrderCommandValidator : AbstractValidator<CreateServiceOrderCommand>
{
    public CreateServiceOrderCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage("O veículo é obrigatório.");

        RuleFor(x => x.RequestDto.OdometerAtService)
            .GreaterThanOrEqualTo(0).WithMessage("O odômetro deve ser maior ou igual a zero.");

        RuleFor(x => x.RequestDto.TotalCost)
            .GreaterThanOrEqualTo(0).WithMessage("O custo total deve ser maior ou igual a zero.");

        RuleFor(x => x.RequestDto.Items)
            .NotEmpty().WithMessage("A ordem de serviço deve conter pelo menos um item (peça ou serviço).");

        RuleForEach(x => x.RequestDto.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Description)
                .NotEmpty().WithMessage("A descrição do item é obrigatória.")
                .MaximumLength(200).WithMessage("A descrição deve ter no máximo 200 caracteres.");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");

            item.RuleFor(i => i.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("O valor unitário deve ser maior ou igual a zero.");

            item.RuleFor(i => i.TotalCost)
                .GreaterThanOrEqualTo(0).WithMessage("O custo total do item deve ser maior ou igual a zero.");
        });
    }
}
