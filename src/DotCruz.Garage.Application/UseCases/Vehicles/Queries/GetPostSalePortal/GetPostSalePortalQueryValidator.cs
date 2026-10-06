using FluentValidation;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetPostSalePortal;

public class GetPostSalePortalQueryValidator : AbstractValidator<GetPostSalePortalQuery>
{
    public GetPostSalePortalQueryValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token do portal pós-venda não pode ser vazio.");
    }
}
