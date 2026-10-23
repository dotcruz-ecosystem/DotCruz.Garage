using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Blueprints.Commands.ApplyBlueprintToVehicle;

public class ApplyBlueprintToVehicleCommandValidator : AbstractValidator<ApplyBlueprintToVehicleCommand>
{
    public ApplyBlueprintToVehicleCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage("O ID do veículo é obrigatório.");

        RuleFor(x => x.RequestDto.Items)
            .NotEmpty().WithMessage("Pelo menos um item deve ser selecionado para aplicar ao veículo.");

        RuleForEach(x => x.RequestDto.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.TemplateId)
                .NotEmpty().WithMessage("O ID do item de template é obrigatório.");

            item.RuleFor(i => i.CalculationMode)
                .Must(mode => mode == "NeverPerformed" || mode == "RecentlyPerformed" || mode == "PendingFirstExecution")
                .WithMessage("Modo de cálculo inválido. Utilize NeverPerformed, RecentlyPerformed ou PendingFirstExecution.");

            item.When(i => i.CalculationMode == "RecentlyPerformed", () =>
            {
                item.RuleFor(i => i.LastPerformedOdometer)
                    .NotNull().WithMessage("O odômetro da última execução é obrigatório para manutenções já realizadas.")
                    .GreaterThanOrEqualTo(0).WithMessage("O odômetro da última execução não pode ser negativo.")
                    .LessThanOrEqualTo(2_000_000).WithMessage("O odômetro não pode exceder 2.000.000 km.");
            });
        });
    }
}
