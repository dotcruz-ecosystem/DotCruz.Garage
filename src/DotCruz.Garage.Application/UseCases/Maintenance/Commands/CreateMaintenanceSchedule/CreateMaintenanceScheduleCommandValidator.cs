using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.CreateMaintenanceSchedule;

public class CreateMaintenanceScheduleCommandValidator : AbstractValidator<CreateMaintenanceScheduleCommand>
{
    public CreateMaintenanceScheduleCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.RequestDto.Description)
            .NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY)
            .MaximumLength(200);

        RuleFor(x => x.RequestDto.Category)
            .IsInEnum();

        RuleFor(x => x)
            .Must(x => x.RequestDto.IntervalOdometer.HasValue || x.RequestDto.IntervalTimeMonths.HasValue)
            .WithMessage(ResourceMessagesException.INTERVAL_REQUIRED);

        RuleFor(x => x.RequestDto.IntervalOdometer)
            .GreaterThan(0).When(x => x.RequestDto.IntervalOdometer.HasValue);

        RuleFor(x => x.RequestDto.IntervalTimeMonths)
            .GreaterThan(0).When(x => x.RequestDto.IntervalTimeMonths.HasValue);

        RuleFor(x => x.RequestDto.WarningThresholdOdometer)
            .GreaterThan(0).When(x => x.RequestDto.WarningThresholdOdometer.HasValue);

        RuleFor(x => x.RequestDto.WarningThresholdDays)
            .GreaterThan(0).When(x => x.RequestDto.WarningThresholdDays.HasValue);
    }
}
