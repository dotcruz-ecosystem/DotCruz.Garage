using DotCruz.Garage.Application.DTOs.Maintenance;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.UpdateMaintenanceSchedule;

public record UpdateMaintenanceScheduleCommand(
    Guid VehicleId,
    Guid ScheduleId,
    UpdateMaintenanceScheduleRequestDto RequestDto) : IRequest;
