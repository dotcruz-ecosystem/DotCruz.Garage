namespace DotCruz.Garage.Application.DTOs.Reports;

public record FleetTcoSummaryDto(
    int TotalVehicles,
    int TotalFleetDistanceKm,
    decimal TotalFleetCost,
    decimal AverageCostPerKm,
    decimal TotalMaintenanceCost,
    decimal TotalFinesCost,
    decimal TotalDocumentsCost,
    decimal TotalAccessoriesCost,
    IReadOnlyCollection<VehicleTcoItemDto> VehicleRanking,
    IReadOnlyCollection<MonthlyTcoDto> MonthlyEvolution);
