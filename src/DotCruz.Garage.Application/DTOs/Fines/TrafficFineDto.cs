namespace DotCruz.Garage.Application.DTOs.Fines;

public record TrafficFineDto(
    Guid Id,
    Guid VehicleId,
    Guid? DriverId,
    string? InfractionCode,
    string? IssuingAuthority,
    string Description,
    int Points,
    DateTimeOffset InfractionDate,
    DateTimeOffset? NotificationDate,
    DateTimeOffset DueDate,
    DateTimeOffset? IndicationDeadline,
    DateTimeOffset? IndicatedAt,
    string? IndicationProtocol,
    decimal Amount,
    decimal? DiscountAmount,
    string Status,
    string? ReceiptUrl,
    int? WarningThresholdDays,
    bool IsNICRisk = false,
    int? DaysUntilIndicationDeadline = null
);
