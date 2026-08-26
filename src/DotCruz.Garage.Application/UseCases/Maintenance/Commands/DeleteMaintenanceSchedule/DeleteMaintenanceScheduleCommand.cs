using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.DeleteMaintenanceSchedule;

public record DeleteMaintenanceScheduleCommand(
    Guid VehicleId,
    Guid ScheduleId) : IRequest;
