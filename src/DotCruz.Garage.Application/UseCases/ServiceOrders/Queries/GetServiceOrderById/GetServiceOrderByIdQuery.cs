using DotCruz.Garage.Application.DTOs.ServiceOrders;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.ServiceOrders.Queries.GetServiceOrderById;

public record GetServiceOrderByIdQuery(Guid Id) : IRequest<ServiceOrderDto>;
