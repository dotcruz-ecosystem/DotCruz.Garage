using DotCruz.Garage.Application.DTOs.Maintenance;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Queries.GetMaintenanceScheduleById;

public record GetMaintenanceScheduleByIdQuery(
    Guid VehicleId,
    Guid ScheduleId) : IRequest<MaintenanceScheduleDto>;
