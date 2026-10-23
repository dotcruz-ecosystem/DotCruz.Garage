using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetCatalogBrands;

public class GetCatalogBrandsQueryHandler : IRequestHandler<GetCatalogBrandsQuery, IReadOnlyCollection<string>>
{
    private readonly IVehicleCatalogRepository _catalogRepository;

    public GetCatalogBrandsQueryHandler(IVehicleCatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public Task<IReadOnlyCollection<string>> Handle(GetCatalogBrandsQuery request, CancellationToken cancellationToken)
    {
        return _catalogRepository.GetBrandsAsync(request.Type, cancellationToken);
    }
}
