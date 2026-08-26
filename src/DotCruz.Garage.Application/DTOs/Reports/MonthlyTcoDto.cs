namespace DotCruz.Garage.Application.DTOs.Reports;

public record MonthlyTcoDto(
    int Year,
    int Month,
    decimal MaintenanceCost,
    decimal FinesCost,
    decimal DocumentsCost,
    decimal AccessoriesCost,
    decimal TotalCost);
