namespace DotCruz.Garage.Application.DTOs.ServiceOrders;

public record ServiceOrderDto(
    Guid Id,
    Guid VehicleId,
    Guid WorkspaceId,
    Guid UserId,
    string? OrderNumber,
    string? WorkshopName,
    DateTimeOffset PerformedAt,
    int OdometerAtService,
    decimal TotalCost,
    decimal Discount,
    string? Notes,
    DateTimeOffset CreatedAt,
    IReadOnlyCollection<ServiceOrderItemDto> Items,
    IReadOnlyCollection<ServiceOrderAttachmentDto> Attachments);
