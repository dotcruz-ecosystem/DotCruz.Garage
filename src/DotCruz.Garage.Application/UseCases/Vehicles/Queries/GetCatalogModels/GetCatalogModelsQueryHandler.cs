using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetCatalogModels;

public class GetCatalogModelsQueryHandler : IRequestHandler<GetCatalogModelsQuery, IReadOnlyCollection<string>>
{
    private readonly IVehicleCatalogRepository _catalogRepository;

    public GetCatalogModelsQueryHandler(IVehicleCatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public Task<IReadOnlyCollection<string>> Handle(GetCatalogModelsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Brand))
        {
            return Task.FromResult<IReadOnlyCollection<string>>(Array.Empty<string>());
        }

        return _catalogRepository.GetModelsAsync(request.Brand, request.Type, cancellationToken);
    }
}
