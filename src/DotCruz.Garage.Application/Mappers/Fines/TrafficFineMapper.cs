using DotCruz.Garage.Application.DTOs.Fines;
using DotCruz.Garage.Application.UseCases.Fines.Commands.CreateTrafficFine;
using DotCruz.Garage.Domain.Entities.Fines;

namespace DotCruz.Garage.Application.Mappers.Fines;

public static class TrafficFineMapper
{
    public static TrafficFine ToDomain(
        this CreateTrafficFineCommand command,
        Guid tenantId,
        Guid userId)
    {
        return TrafficFine.Create(
            tenantId,
            userId,
            command.VehicleId,
            command.RequestDto.InfractionCode,
            command.RequestDto.Description,
            command.RequestDto.InfractionDate,
            command.RequestDto.NotificationDate,
            command.RequestDto.DueDate,
            command.RequestDto.Amount,
            command.RequestDto.DiscountAmount,
            command.RequestDto.Status,
            command.RequestDto.ReceiptUrl,
            command.RequestDto.WarningThresholdDays
        );
    }

    public static TrafficFineDto ToDto(this TrafficFine fine)
    {
        return new TrafficFineDto(
            fine.Id,
            fine.VehicleId,
            fine.InfractionCode,
            fine.Description,
            fine.InfractionDate,
            fine.NotificationDate,
            fine.DueDate,
            fine.Amount,
            fine.DiscountAmount,
            fine.Status.ToString(),
            fine.ReceiptUrl,
            fine.WarningThresholdDays
        );
    }
}
