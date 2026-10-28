using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.DTOs.ServiceOrders;

public record ExtractedServiceOrderItemDto(
    string Description,
    ServiceOrderItemType Type,
    MaintenanceCategory Category,
    decimal Quantity,
    decimal UnitPrice,
    decimal TotalCost,
    string? PartNumber = null,
    Guid? MatchedMaintenanceScheduleId = null,
    string? Notes = null);
