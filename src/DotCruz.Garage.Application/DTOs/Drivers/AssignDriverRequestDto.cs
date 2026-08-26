namespace DotCruz.Garage.Application.DTOs.Drivers;

public record AssignDriverRequestDto(
    Guid VehicleId,
    DateTimeOffset? StartedAt = null,
    string? Notes = null);
