using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Domain.Entities.ServiceOrders;

public class ServiceOrderItem : BaseEntity
{
    public Guid ServiceOrderId { get; private set; }
    public Guid? MaintenanceScheduleId { get; private set; }
    public string Description { get; private set; } = null!;
    public ServiceOrderItemType Type { get; private set; }
    public MaintenanceCategory Category { get; private set; }
    public string? PartNumber { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalCost { get; private set; }
    public string? Notes { get; private set; }

    public virtual ServiceOrder ServiceOrder { get; private init; } = default!;
    public virtual MaintenanceSchedule? MaintenanceSchedule { get; private init; }

    private ServiceOrderItem() { }

    public static ServiceOrderItem Create(
        Guid serviceOrderId,
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
        return new ServiceOrderItem
        {
            ServiceOrderId = serviceOrderId,
            Description = description.Trim(),
            Type = type,
            Category = category,
            Quantity = quantity <= 0 ? 1 : quantity,
            UnitPrice = unitPrice,
            TotalCost = totalCost,
            MaintenanceScheduleId = maintenanceScheduleId,
            PartNumber = string.IsNullOrWhiteSpace(partNumber) ? null : partNumber.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}
