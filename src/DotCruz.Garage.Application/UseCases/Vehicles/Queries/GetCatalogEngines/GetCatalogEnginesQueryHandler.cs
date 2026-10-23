using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetCatalogEngines;

public class GetCatalogEnginesQueryHandler : IRequestHandler<GetCatalogEnginesQuery, IReadOnlyCollection<string>>
{
    private readonly IVehicleCatalogRepository _catalogRepository;

    public GetCatalogEnginesQueryHandler(IVehicleCatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public Task<IReadOnlyCollection<string>> Handle(GetCatalogEnginesQuery request, CancellationToken cancellationToken)
    {
        return _catalogRepository.GetEnginesAsync(request.Type, cancellationToken);
    }
}
