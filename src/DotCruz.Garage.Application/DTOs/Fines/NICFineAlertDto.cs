namespace DotCruz.Garage.Application.DTOs.Fines;

public record NICFineAlertDto(
    Guid FineId,
    Guid VehicleId,
    string VehicleName,
    string? InfractionCode,
    string Description,
    decimal Amount,
    DateTimeOffset InfractionDate,
    DateTimeOffset? IndicationDeadline,
    int? DaysUntilDeadline,
    bool IsUrgentRisk);
