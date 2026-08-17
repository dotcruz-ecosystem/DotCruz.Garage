using DotCruz.Garage.Application.Abstractions.Data.Repositories.Accessories;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.DTOs.Accessories;
using DotCruz.Garage.Application.Mappers.Accessories;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Accessories.Queries.SearchVehicleAccessories;

public class SearchVehicleAccessoriesQueryHandler : IRequestHandler<SearchVehicleAccessoriesQuery, SearchVehicleAccessoriesResponseDto>
{
    private readonly IVehicleAccessoryReadRepository _repository;
    private readonly IVehicleReadRepository _vehicleRepository;

    public SearchVehicleAccessoriesQueryHandler(
        IVehicleAccessoryReadRepository repository,
        IVehicleReadRepository vehicleRepository)
    {
        _repository = repository;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<SearchVehicleAccessoriesResponseDto> Handle(SearchVehicleAccessoriesQuery request, CancellationToken cancellationToken)
    {
        if (request.VehicleId.HasValue)
        {
            var vehicleExists = await _vehicleRepository.ExistsAsync(request.VehicleId.Value, cancellationToken);
            if (!vehicleExists)
            {
                throw new NotFoundException("Vehicle not found.");
            }
        }

        var (items, totalCount) = await _repository.SearchAsync(
            request.PageNumber,
            request.PageSize,
            request.VehicleId,
            cancellationToken
        );

        var dtos = items.Select(a => a.ToDto()).ToList();

        return new SearchVehicleAccessoriesResponseDto(
            dtos,
            request.PageNumber,
            request.PageSize,
            totalCount
        );
    }
}
