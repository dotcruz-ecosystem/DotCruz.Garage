using DotCruz.Garage.Application.Abstractions.Data.Repositories.ServiceOrders;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.DTOs.ServiceOrders;
using DotCruz.Garage.Application.Mappers.ServiceOrders;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.ServiceOrders.Queries.GetServiceOrdersByVehicle;

public class GetServiceOrdersByVehicleQueryHandler : IRequestHandler<GetServiceOrdersByVehicleQuery, SearchServiceOrdersResponseDto>
{
    private readonly IVehicleReadRepository _vehicleReadRepository;
    private readonly IServiceOrderReadRepository _serviceOrderReadRepository;

    public GetServiceOrdersByVehicleQueryHandler(
        IVehicleReadRepository vehicleReadRepository,
        IServiceOrderReadRepository serviceOrderReadRepository)
    {
        _vehicleReadRepository = vehicleReadRepository;
        _serviceOrderReadRepository = serviceOrderReadRepository;
    }

    public async Task<SearchServiceOrdersResponseDto> Handle(GetServiceOrdersByVehicleQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleReadRepository.GetByIdAsync(request.VehicleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var (items, totalCount) = await _serviceOrderReadRepository.GetByVehicleIdPaginatedAsync(
            vehicle.Id,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(x => x.ToDto()).ToList();

        return new SearchServiceOrdersResponseDto(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}
