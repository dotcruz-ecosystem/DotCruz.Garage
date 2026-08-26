using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Reports.Queries.GetFleetTcoSummary;

public class GetFleetTcoSummaryQueryValidator : AbstractValidator<GetFleetTcoSummaryQuery>
{
    public GetFleetTcoSummaryQueryValidator()
    {
        RuleFor(x => x)
            .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.StartDate <= x.EndDate)
            .WithMessage("A data inicial deve ser menor ou igual à data final.");
    }
}
