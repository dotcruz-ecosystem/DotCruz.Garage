namespace DotCruz.Garage.Application.DTOs.ServiceOrders;

public record CreateServiceOrderAttachmentRequestDto(
    string FileName,
    string FileUrl,
    string ContentType,
    long FileSize);
