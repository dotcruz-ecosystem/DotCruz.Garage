using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.DTOs.Documents;

public sealed record CreateVehicleDocumentRequestDto(
    DocumentType Type,
    int ReferenceYear,
    DateTimeOffset? DueDate,
    decimal Cost,
    DateTimeOffset? ExpirationDate,
    DocumentStatus Status,
    string? FileUrl,
    int? WarningThresholdDays
);
