using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Accessories.Queries.SearchVehicleAccessories;

public class SearchVehicleAccessoriesQueryValidator : AbstractValidator<SearchVehicleAccessoriesQuery>
{
    public SearchVehicleAccessoriesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}
