using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.LogMaintenance;

public class LogMaintenanceCommandValidator : AbstractValidator<LogMaintenanceCommand>
{
    public LogMaintenanceCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.RequestDto.Description)
            .NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY)
            .MaximumLength(500);

        RuleFor(x => x.RequestDto.OdometerAtService)
            .GreaterThanOrEqualTo(0).WithMessage(ResourceMessagesException.ODOMETER_NEGATIVE);

        RuleFor(x => x.RequestDto.Cost)
            .GreaterThanOrEqualTo(0);
    }
}
