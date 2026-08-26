using DotCruz.Garage.Application.DTOs.Drivers;
using DotCruz.Garage.Application.DTOs.Maintenance;

namespace DotCruz.Garage.Application.DTOs.Reports;

public record VehicleFinancialSummaryDto(
    decimal TotalMaintenanceCost,
    decimal TotalFinesCost,
    decimal TotalDocumentsCost,
    decimal TotalAccessoriesCost,
    decimal TotalTco,
    decimal CostPerKm);

public record PassportMaintenanceLogDto(
    Guid Id,
    DateTimeOffset PerformedAt,
    int OdometerAtService,
    string Description,
    decimal Cost,
    string? ReceiptUrl,
    string? Notes);

public record PassportMaintenanceScheduleDto(
    Guid Id,
    string Description,
    string Category,
    int? NextDueOdometer,
    DateTimeOffset? NextDueDate,
    string Status);

public record PassportDocumentDto(
    Guid Id,
    string DocumentType,
    int ReferenceYear,
    decimal Cost,
    DateTimeOffset? ExpirationDate,
    string Status);

public record VehiclePassportDto(
    Guid VehicleId,
    string Name,
    string Brand,
    string Model,
    int Year,
    string? LicensePlate,
    string? Vin,
    int CurrentOdometer,
    int TotalDistanceTraveledKm,
    double DailyMileageAverage,
    string? CurrentDriverName,
    VehicleFinancialSummaryDto FinancialSummary,
    IReadOnlyCollection<PassportMaintenanceLogDto> MaintenanceHistory,
    IReadOnlyCollection<PassportMaintenanceScheduleDto> UpcomingMaintenances,
    IReadOnlyCollection<PassportDocumentDto> Documents,
    DateTimeOffset GeneratedAt,
    string VerificationCode);
