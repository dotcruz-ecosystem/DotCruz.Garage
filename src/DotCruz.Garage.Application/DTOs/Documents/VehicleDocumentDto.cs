using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.DTOs.Documents;

public record VehicleDocumentDto(
    Guid Id,
    Guid VehicleId,
    DocumentType Type,
    int ReferenceYear,
    DateTimeOffset? DueDate,
    decimal Cost,
    DateTimeOffset? ExpirationDate,
    string Status,
    string? FileUrl,
    int? WarningThresholdDays
);
