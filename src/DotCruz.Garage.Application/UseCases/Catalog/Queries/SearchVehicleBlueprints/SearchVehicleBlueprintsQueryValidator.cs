using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Catalog.Queries.SearchVehicleBlueprints;

public class SearchVehicleBlueprintsQueryValidator : AbstractValidator<SearchVehicleBlueprintsQuery>
{
    public SearchVehicleBlueprintsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage(ResourceMessagesException.PAGE_NUMBER_INVALID);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage(ResourceMessagesException.PAGE_SIZE_INVALID);
    }
}
