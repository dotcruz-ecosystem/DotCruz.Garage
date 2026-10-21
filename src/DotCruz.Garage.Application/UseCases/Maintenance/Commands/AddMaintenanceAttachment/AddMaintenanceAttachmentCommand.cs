using DotCruz.Garage.Application.DTOs.Maintenance;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.AddMaintenanceAttachment;

public record AddMaintenanceAttachmentCommand(
    Guid VehicleId,
    Guid MaintenanceLogId,
    CreateMaintenanceAttachmentDto RequestDto
) : IRequest<MaintenanceAttachmentDto>;
