using DotCruz.Garage.Application.Abstractions.Data.Repositories.Catalog;
using DotCruz.Garage.Application.DTOs.Catalog;
using DotCruz.Garage.Application.Mappers.Catalog;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Catalog.Queries.SearchVehicleBlueprints;

public class SearchVehicleBlueprintsQueryHandler : IRequestHandler<SearchVehicleBlueprintsQuery, SearchVehicleBlueprintsResponseDto>
{
    private readonly IVehicleBlueprintReadRepository _blueprintReadRepository;

    public SearchVehicleBlueprintsQueryHandler(IVehicleBlueprintReadRepository blueprintReadRepository)
    {
        _blueprintReadRepository = blueprintReadRepository;
    }

    public async Task<SearchVehicleBlueprintsResponseDto> Handle(SearchVehicleBlueprintsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _blueprintReadRepository.SearchAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            request.Brand,
            request.Model,
            request.Year,
            cancellationToken);

        var dtos = items.Select(b => b.ToDto()).ToList();

        return new SearchVehicleBlueprintsResponseDto(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}
