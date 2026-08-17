using DotCruz.Garage.Application.DTOs.Maintenance;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.CreateMaintenanceSchedule;

public sealed record CreateMaintenanceScheduleCommand(
    Guid VehicleId,
    CreateMaintenanceScheduleRequestDto RequestDto
) : IRequest<Guid>;
