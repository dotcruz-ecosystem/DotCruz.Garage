using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.DTOs.Fines;

public sealed record CreateTrafficFineRequestDto(
    string? InfractionCode,
    string Description,
    DateTimeOffset InfractionDate,
    DateTimeOffset? NotificationDate,
    DateTimeOffset DueDate,
    decimal Amount,
    decimal? DiscountAmount,
    FineStatus Status,
    string? ReceiptUrl,
    int? WarningThresholdDays
);
