using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Fines.Queries.SearchTrafficFines;

public class SearchTrafficFinesQueryValidator : AbstractValidator<SearchTrafficFinesQuery>
{
    public SearchTrafficFinesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}
