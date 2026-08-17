using DotCruz.Garage.Application.Abstractions.Data.Repositories.Fines;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.DTOs.Fines;
using DotCruz.Garage.Application.Mappers.Fines;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Fines.Queries.SearchTrafficFines;

public class SearchTrafficFinesQueryHandler : IRequestHandler<SearchTrafficFinesQuery, SearchTrafficFinesResponseDto>
{
    private readonly ITrafficFineReadRepository _repository;
    private readonly IVehicleReadRepository _vehicleRepository;

    public SearchTrafficFinesQueryHandler(
        ITrafficFineReadRepository repository,
        IVehicleReadRepository vehicleRepository)
    {
        _repository = repository;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<SearchTrafficFinesResponseDto> Handle(SearchTrafficFinesQuery request, CancellationToken cancellationToken)
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

        var dtos = items.Select(f => f.ToDto()).ToList();

        return new SearchTrafficFinesResponseDto(
            dtos,
            request.PageNumber,
            request.PageSize,
            totalCount
        );
    }
}
