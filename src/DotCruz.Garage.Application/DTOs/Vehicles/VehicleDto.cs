namespace DotCruz.Garage.Application.DTOs.Vehicles;

public record VehicleDto(
    Guid Id,
    string Name,
    string Brand,
    string Model,
    int Year,
    string? Plate,
    string? Vin,
    int CurrentOdometer,
    double DailyMileageAverage,
    DateTimeOffset? OdometerUpdatedAt
);
