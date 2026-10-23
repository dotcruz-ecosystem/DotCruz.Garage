using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.DTOs.ServiceOrders;

public record CreateServiceOrderItemRequestDto(
    string Description,
    ServiceOrderItemType Type,
    MaintenanceCategory Category,
    decimal Quantity,
    decimal UnitPrice,
    decimal TotalCost,
    Guid? MaintenanceScheduleId = null,
    string? PartNumber = null,
    string? Notes = null);
