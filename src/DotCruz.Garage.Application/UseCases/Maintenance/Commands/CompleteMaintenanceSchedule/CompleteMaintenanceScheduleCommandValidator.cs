using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.CompleteMaintenanceSchedule;

public class CompleteMaintenanceScheduleCommandValidator : AbstractValidator<CompleteMaintenanceScheduleCommand>
{
    public CompleteMaintenanceScheduleCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.ScheduleId)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.RequestDto.OdometerAtService)
            .GreaterThan(0)
            .WithMessage(ResourceMessagesException.ODOMETER_NEGATIVE);

        RuleFor(x => x.RequestDto.Cost)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ResourceMessagesException.COST_NEGATIVE);
    }
}
