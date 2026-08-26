namespace DotCruz.Garage.Application.DTOs.Drivers;

public record VehicleDriverAssignmentDto(
    Guid Id,
    Guid VehicleId,
    Guid DriverId,
    string? DriverName,
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt,
    bool IsCurrent,
    string? Notes);
