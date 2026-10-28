namespace DotCruz.Garage.Application.DTOs.ServiceOrders;

public record ExtractServiceOrderFromAttachmentRequestDto(
    string FileUrl,
    string FileName,
    string ContentType);
