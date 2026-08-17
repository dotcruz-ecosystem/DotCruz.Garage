using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.DTOs.Maintenance;
using DotCruz.Garage.Application.Mappers.Maintenance;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Queries.SearchMaintenanceSchedules;

public class SearchMaintenanceSchedulesQueryHandler : IRequestHandler<SearchMaintenanceSchedulesQuery, SearchMaintenanceSchedulesResponseDto>
{
    private readonly IMaintenanceScheduleReadRepository _repository;
    private readonly IVehicleReadRepository _vehicleRepository;

    public SearchMaintenanceSchedulesQueryHandler(
        IMaintenanceScheduleReadRepository repository,
        IVehicleReadRepository vehicleRepository)
    {
        _repository = repository;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<SearchMaintenanceSchedulesResponseDto> Handle(SearchMaintenanceSchedulesQuery request, CancellationToken cancellationToken)
    {
        var vehicleExists = await _vehicleRepository.ExistsAsync(request.VehicleId, cancellationToken);
        if (!vehicleExists)
        {
            throw new NotFoundException("Vehicle not found.");
        }

        var (items, totalCount) = await _repository.SearchAsync(
            request.PageNumber,
            request.PageSize,
            request.VehicleId,
            cancellationToken
        );

        var dtos = items.Select(s => s.ToDto()).ToList();

        return new SearchMaintenanceSchedulesResponseDto(
            dtos,
            request.PageNumber,
            request.PageSize,
            totalCount
        );
    }
}
