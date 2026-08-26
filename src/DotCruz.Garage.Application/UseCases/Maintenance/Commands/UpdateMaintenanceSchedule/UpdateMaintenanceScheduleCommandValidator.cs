using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.UpdateMaintenanceSchedule;

public class UpdateMaintenanceScheduleCommandValidator : AbstractValidator<UpdateMaintenanceScheduleCommand>
{
    public UpdateMaintenanceScheduleCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.ScheduleId)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.RequestDto.Description)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.NAME_EMPTY);
    }
}
