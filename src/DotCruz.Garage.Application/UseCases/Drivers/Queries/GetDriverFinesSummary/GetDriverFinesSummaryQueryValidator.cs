using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Drivers.Queries.GetDriverFinesSummary;

public class GetDriverFinesSummaryQueryValidator : AbstractValidator<GetDriverFinesSummaryQuery>
{
    public GetDriverFinesSummaryQueryValidator()
    {
        RuleFor(x => x.DriverId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);
    }
}
