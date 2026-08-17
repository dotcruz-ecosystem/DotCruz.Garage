using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Application.Mappers.Vehicles;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Queries.SearchVehicles;

public class SearchVehiclesQueryHandler : IRequestHandler<SearchVehiclesQuery, SearchVehiclesResponseDto>
{
    private readonly IVehicleReadRepository _vehicleReadRepository;

    public SearchVehiclesQueryHandler(IVehicleReadRepository vehicleReadRepository)
    {
        _vehicleReadRepository = vehicleReadRepository;
    }

    public async Task<SearchVehiclesResponseDto> Handle(SearchVehiclesQuery request, CancellationToken cancellationToken)
    {
        var (vehicles, totalCount) = await _vehicleReadRepository.SearchAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            cancellationToken
        );

        var vehicleDtos = vehicles.Select(v => v.ToDto()).ToList();

        return new SearchVehiclesResponseDto(
            vehicleDtos,
            request.PageNumber,
            request.PageSize,
            totalCount
        );
    }
}
