using DotCruz.Garage.Application.DTOs.ServiceOrders;
using DotCruz.Garage.Domain.Entities.ServiceOrders;

namespace DotCruz.Garage.Application.Mappers.ServiceOrders;

public static class ServiceOrderMapper
{
    public static ServiceOrderDto ToDto(this ServiceOrder entity)
    {
        return new ServiceOrderDto(
            entity.Id,
            entity.VehicleId,
            entity.WorkspaceId,
            entity.UserId,
            entity.OrderNumber,
            entity.WorkshopName,
            entity.PerformedAt,
            entity.OdometerAtService,
            entity.TotalCost,
            entity.Discount,
            entity.Notes,
            entity.CreatedAt,
            entity.Items.Select(i => i.ToDto()).ToList(),
            entity.Attachments.Select(a => a.ToDto()).ToList());
    }

    public static ServiceOrderItemDto ToDto(this ServiceOrderItem entity)
    {
        return new ServiceOrderItemDto(
            entity.Id,
            entity.ServiceOrderId,
            entity.MaintenanceScheduleId,
            entity.Description,
            entity.Type,
            entity.Category,
            entity.PartNumber,
            entity.Quantity,
            entity.UnitPrice,
            entity.TotalCost,
            entity.Notes);
    }

    public static ServiceOrderAttachmentDto ToDto(this ServiceOrderAttachment entity)
    {
        return new ServiceOrderAttachmentDto(
            entity.Id,
            entity.ServiceOrderId,
            entity.FileName,
            entity.FileUrl,
            entity.ContentType,
            entity.FileSize,
            entity.CreatedAt);
    }
}
