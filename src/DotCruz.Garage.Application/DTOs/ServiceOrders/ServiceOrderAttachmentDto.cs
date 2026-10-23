namespace DotCruz.Garage.Application.DTOs.ServiceOrders;

public record ServiceOrderAttachmentDto(
    Guid Id,
    Guid ServiceOrderId,
    string FileName,
    string FileUrl,
    string ContentType,
    long FileSize,
    DateTimeOffset CreatedAt);
