namespace DotCruz.Garage.Application.DTOs.Reports;

public record VehicleTcoItemDto(
    Guid VehicleId,
    string VehicleName,
    string LicensePlate,
    int CurrentOdometer,
    int DistanceTraveledKm,
    decimal MaintenanceCost,
    decimal FinesCost,
    decimal DocumentsCost,
    decimal AccessoriesCost,
    decimal TotalCost,
    decimal CostPerKm);
