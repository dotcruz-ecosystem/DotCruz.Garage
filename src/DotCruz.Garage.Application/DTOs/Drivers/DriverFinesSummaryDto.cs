using DotCruz.Garage.Application.DTOs.Fines;

namespace DotCruz.Garage.Application.DTOs.Drivers;

public record DriverFinesSummaryDto(
    Guid DriverId,
    string DriverName,
    string Cpf,
    string Cnh,
    int TotalPointsLast12Months,
    bool IsSuspensionRisk,
    int TotalFinesCount,
    decimal TotalFinesAmount,
    IReadOnlyCollection<TrafficFineDto> Fines);
