using DotCruz.Garage.Application.DTOs.ServiceOrders;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.ServiceOrders.Commands.CreateServiceOrder;

public record CreateServiceOrderCommand(
    Guid VehicleId,
    CreateServiceOrderRequestDto RequestDto) : IRequest<ServiceOrderDto>;
