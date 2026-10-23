namespace DotCruz.Garage.Application.DTOs.ServiceOrders;

public record CreateServiceOrderRequestDto(
    string? OrderNumber,
    string? WorkshopName,
    DateTimeOffset PerformedAt,
    int OdometerAtService,
    decimal TotalCost,
    decimal Discount,
    string? Notes,
    List<CreateServiceOrderItemRequestDto> Items,
    List<CreateServiceOrderAttachmentRequestDto>? Attachments = null);
