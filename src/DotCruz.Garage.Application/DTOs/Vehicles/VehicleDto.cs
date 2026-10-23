using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.DTOs.Vehicles;

public record VehicleDto(
    Guid Id,
    string Name,
    VehicleType Type,
    string Brand,
    string Model,
    string Engine,
    int Year,
    string? Plate,
    int CurrentOdometer,
    double DailyMileageAverage,
    DateTimeOffset? OdometerUpdatedAt
);
