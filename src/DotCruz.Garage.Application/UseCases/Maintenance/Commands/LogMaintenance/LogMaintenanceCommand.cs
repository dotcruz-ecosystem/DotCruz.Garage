using DotCruz.Garage.Application.DTOs.Maintenance;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.LogMaintenance;

public sealed record LogMaintenanceCommand(
    Guid VehicleId,
    CreateLogMaintenanceRequestDto RequestDto
) : IRequest<Guid>;
