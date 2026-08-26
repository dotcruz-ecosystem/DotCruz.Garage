using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Reports.Queries.GetVehiclePassport;

public class GetVehiclePassportQueryValidator : AbstractValidator<GetVehiclePassportQuery>
{
    public GetVehiclePassportQueryValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);
    }
}
