using DotCruz.Garage.Application.DTOs.ServiceOrders;

namespace DotCruz.Garage.Application.DTOs.Maintenance;

public record CompleteMaintenanceScheduleRequestDto(
    DateTimeOffset PerformedAt,
    int OdometerAtService,
    decimal Cost,
    string? ReceiptUrl = null,
    string? Notes = null,
    List<CreateServiceOrderAttachmentRequestDto>? Attachments = null);
