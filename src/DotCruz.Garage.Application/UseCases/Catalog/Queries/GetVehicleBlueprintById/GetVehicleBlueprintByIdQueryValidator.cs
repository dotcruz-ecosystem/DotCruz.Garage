using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Catalog.Queries.GetVehicleBlueprintById;

public class GetVehicleBlueprintByIdQueryValidator : AbstractValidator<GetVehicleBlueprintByIdQuery>
{
    public GetVehicleBlueprintByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.ID_EMPTY);
    }
}
