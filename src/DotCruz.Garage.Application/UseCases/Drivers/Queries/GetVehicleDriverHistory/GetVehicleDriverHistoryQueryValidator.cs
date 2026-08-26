using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Drivers.Queries.GetVehicleDriverHistory;

public class GetVehicleDriverHistoryQueryValidator : AbstractValidator<GetVehicleDriverHistoryQuery>
{
    public GetVehicleDriverHistoryQueryValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);
    }
}
