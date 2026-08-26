using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Queries.GetMaintenanceScheduleById;

public class GetMaintenanceScheduleByIdQueryValidator : AbstractValidator<GetMaintenanceScheduleByIdQuery>
{
    public GetMaintenanceScheduleByIdQueryValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.ID_EMPTY);

        RuleFor(x => x.ScheduleId)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.ID_EMPTY);
    }
}
