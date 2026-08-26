using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Queries.GetVehicleMaintenanceOverview;

public class GetVehicleMaintenanceOverviewQueryValidator : AbstractValidator<GetVehicleMaintenanceOverviewQuery>
{
    public GetVehicleMaintenanceOverviewQueryValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.ID_EMPTY);
    }
}
