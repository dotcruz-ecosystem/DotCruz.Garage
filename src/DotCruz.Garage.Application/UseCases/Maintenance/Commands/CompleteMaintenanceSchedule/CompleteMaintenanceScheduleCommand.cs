using DotCruz.Garage.Application.DTOs.Maintenance;
using DotCruz.Garage.Application.DTOs.ServiceOrders;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.CompleteMaintenanceSchedule;

public record CompleteMaintenanceScheduleCommand(
    Guid VehicleId,
    Guid ScheduleId,
    CompleteMaintenanceScheduleRequestDto RequestDto) : IRequest<ServiceOrderDto>;
