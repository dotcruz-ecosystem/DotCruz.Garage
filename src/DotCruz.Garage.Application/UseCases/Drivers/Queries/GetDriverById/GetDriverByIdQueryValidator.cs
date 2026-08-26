using DotCruz.Garage.Domain.Exceptions.Resources;
using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Drivers.Queries.GetDriverById;

public class GetDriverByIdQueryValidator : AbstractValidator<GetDriverByIdQuery>
{
    public GetDriverByIdQueryValidator()
    {
        RuleFor(x => x.DriverId)
            .NotEmpty().WithMessage(ResourceMessagesException.ID_EMPTY);
    }
}
