using DotCruz.Garage.Application.DTOs.Maintenance;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.LogMaintenance;
using DotCruz.Garage.Domain.Entities.Maintenance;

namespace DotCruz.Garage.Application.Mappers.Maintenance;

public static class MaintenanceLogMapper
{
    public static MaintenanceLog ToDomain(
        this LogMaintenanceCommand command,
        Guid tenantId,
        Guid userId)
    {
        return MaintenanceLog.Create(
            tenantId,
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
    }

    public static MaintenanceLogDto ToDto(this MaintenanceLog log)
    {
        return new MaintenanceLogDto(
            log.Id,
            log.VehicleId,
            log.MaintenanceScheduleId,
            log.Description,
            log.PerformedAt,
            log.OdometerAtService,
            log.Cost,
            log.ReceiptUrl,
            log.Notes
        );
    }
}
