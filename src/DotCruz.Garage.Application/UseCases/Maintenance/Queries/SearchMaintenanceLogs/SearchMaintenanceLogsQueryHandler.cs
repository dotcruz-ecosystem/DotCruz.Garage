using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.DTOs.Maintenance;
using DotCruz.Garage.Application.Mappers.Maintenance;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;

using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Queries.SearchMaintenanceLogs;

public class SearchMaintenanceLogsQueryHandler : IRequestHandler<SearchMaintenanceLogsQuery, SearchMaintenanceLogsResponseDto>
{
    private readonly IMaintenanceLogReadRepository _repository;
    private readonly IVehicleReadRepository _vehicleRepository;

    public SearchMaintenanceLogsQueryHandler(
        IMaintenanceLogReadRepository repository,
        IVehicleReadRepository vehicleRepository)
    {
        _repository = repository;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<SearchMaintenanceLogsResponseDto> Handle(SearchMaintenanceLogsQuery request, CancellationToken cancellationToken)
    {
        var vehicleExists = await _vehicleRepository.ExistsAsync(request.VehicleId, cancellationToken);
        if (!vehicleExists)
        {
            throw new NotFoundException("Vehicle not found.");
        }

        var (items, totalCount) = await _repository.GetByVehicleIdPaginatedAsync(
            request.VehicleId,
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        var dtos = items.Select(l => l.ToDto()).ToList();

        return new SearchMaintenanceLogsResponseDto(
            dtos,
            request.PageNumber,
            request.PageSize,
            totalCount
        );
    }
}
