using DotCruz.Garage.Application.DTOs.ServiceOrders;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.ServiceOrders.Queries.GetServiceOrdersByVehicle;

public record GetServiceOrdersByVehicleQuery(
    Guid VehicleId,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<SearchServiceOrdersResponseDto>;
