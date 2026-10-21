using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.RemoveMaintenanceAttachment;

public record RemoveMaintenanceAttachmentCommand(
    Guid VehicleId,
    Guid MaintenanceLogId,
    Guid AttachmentId
) : IRequest;
