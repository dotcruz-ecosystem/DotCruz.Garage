using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.DTOs.ServiceOrders;

public record ServiceOrderItemDto(
    Guid Id,
    Guid ServiceOrderId,
    Guid? MaintenanceScheduleId,
    string Description,
    ServiceOrderItemType Type,
    MaintenanceCategory Category,
    string? PartNumber,
    decimal Quantity,
    decimal UnitPrice,
    decimal TotalCost,
    string? Notes);
