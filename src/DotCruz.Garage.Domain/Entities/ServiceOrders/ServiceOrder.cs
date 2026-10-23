using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Domain.Entities.ServiceOrders;

public class ServiceOrder : BaseEntity, IWorkspaceEntity
{
    public Guid WorkspaceId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid VehicleId { get; private set; }
    public string? OrderNumber { get; private set; }
    public string? WorkshopName { get; private set; }
    public DateTimeOffset PerformedAt { get; private set; }
    public int OdometerAtService { get; private set; }
    public decimal TotalCost { get; private set; }
    public decimal Discount { get; private set; }
    public string? Notes { get; private set; }

    public virtual Vehicle Vehicle { get; private init; } = default!;
    public virtual ICollection<ServiceOrderItem> Items { get; private init; } = new List<ServiceOrderItem>();
    public virtual ICollection<ServiceOrderAttachment> Attachments { get; private init; } = new List<ServiceOrderAttachment>();

    private ServiceOrder() { }

    public static ServiceOrder Create(
        Guid workspaceId,
        Guid userId,
        Guid vehicleId,
        string? orderNumber,
        string? workshopName,
        DateTimeOffset performedAt,
        int odometerAtService,
        decimal totalCost,
        decimal discount = 0,
        string? notes = null)
    {
        return new ServiceOrder
        {
            WorkspaceId = workspaceId,
            UserId = userId,
            VehicleId = vehicleId,
            OrderNumber = string.IsNullOrWhiteSpace(orderNumber) ? null : orderNumber.Trim(),
            WorkshopName = string.IsNullOrWhiteSpace(workshopName) ? null : workshopName.Trim(),
            PerformedAt = performedAt,
            OdometerAtService = odometerAtService,
            TotalCost = totalCost,
            Discount = discount,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }

    public ServiceOrderItem AddItem(
        string description,
        ServiceOrderItemType type,
        MaintenanceCategory category,
        decimal quantity,
        decimal unitPrice,
        decimal totalCost,
        Guid? maintenanceScheduleId = null,
        string? partNumber = null,
        string? notes = null)
    {
        var item = ServiceOrderItem.Create(
            Id,
            description,
            type,
            category,
            quantity,
            unitPrice,
            totalCost,
            maintenanceScheduleId,
            partNumber,
            notes);

        Items.Add(item);
        Touch();
        return item;
    }

    public ServiceOrderAttachment AddAttachment(string fileName, string fileUrl, string contentType, long fileSize)
    {
        var attachment = ServiceOrderAttachment.Create(Id, fileName, fileUrl, contentType, fileSize);
        Attachments.Add(attachment);
        Touch();
        return attachment;
    }

    public void RemoveAttachment(Guid attachmentId)
    {
        var attachment = Attachments.FirstOrDefault(a => a.Id == attachmentId);
        if (attachment != null)
        {
            Attachments.Remove(attachment);
            Touch();
        }
    }
}
