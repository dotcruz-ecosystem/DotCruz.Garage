using DotCruz.Garage.Application.DTOs.Maintenance;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.LogMaintenance;
using DotCruz.Garage.Domain.Entities.Maintenance;

namespace DotCruz.Garage.Application.Mappers.Maintenance;

public static class MaintenanceLogMapper
{
    public static MaintenanceLog ToDomain(
        this LogMaintenanceCommand command,
        Guid workspaceId,
        Guid userId)
    {
        var log = MaintenanceLog.Create(
            workspaceId,
            userId,
            command.VehicleId,
            command.RequestDto.MaintenanceScheduleId,
            command.RequestDto.Description,
            command.RequestDto.PerformedAt,
            command.RequestDto.OdometerAtService,
            command.RequestDto.Cost,
            command.RequestDto.ReceiptUrl,
            command.RequestDto.Notes
        );

        if (command.RequestDto.Attachments != null)
        {
            foreach (var att in command.RequestDto.Attachments)
            {
                log.AddAttachment(att.FileName, att.FileUrl, att.ContentType, att.FileSize);
            }
        }

        return log;
    }

    public static MaintenanceLogDto ToDto(this MaintenanceLog log)
    {
        var attachmentsDto = log.Attachments?
            .Select(a => new MaintenanceAttachmentDto(
                a.Id,
                a.MaintenanceLogId,
                a.FileName,
                a.FileUrl,
                a.ContentType,
                a.FileSize,
                a.CreatedAt))
            .ToList() ?? [];

        return new MaintenanceLogDto(
            log.Id,
            log.VehicleId,
            log.MaintenanceScheduleId,
            log.Description,
            log.PerformedAt,
            log.OdometerAtService,
            log.Cost,
            log.ReceiptUrl,
            log.Notes,
            log.Type.ToString(),
            attachmentsDto
        );
    }
}
