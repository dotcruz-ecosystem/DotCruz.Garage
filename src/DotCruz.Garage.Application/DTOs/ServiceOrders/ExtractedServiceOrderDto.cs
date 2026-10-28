namespace DotCruz.Garage.Application.DTOs.ServiceOrders;

public record ExtractedServiceOrderDto(
    string? OrderNumber,
    string? WorkshopName,
    DateTimeOffset? PerformedAt,
    int? OdometerAtService,
    decimal TotalCost,
    decimal Discount,
    string? Notes,
    string? DetectedVehiclePlate,
    string? DetectedVehicleModel,
    List<ExtractedServiceOrderItemDto> Items,
    List<string> Warnings);
