namespace DotCruz.Garage.Application.DTOs.Fines;

public record TrafficFineDto(
    Guid Id,
    Guid VehicleId,
    string? InfractionCode,
    string Description,
    DateTimeOffset InfractionDate,
    DateTimeOffset? NotificationDate,
    DateTimeOffset DueDate,
    decimal Amount,
    decimal? DiscountAmount,
    string Status,
    string? ReceiptUrl,
    int? WarningThresholdDays
);
