using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.DeleteMaintenanceSchedule;

public class DeleteMaintenanceScheduleCommandValidator : AbstractValidator<DeleteMaintenanceScheduleCommand>
{
    public DeleteMaintenanceScheduleCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.ScheduleId)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.ID_EMPTY);
    }
}
