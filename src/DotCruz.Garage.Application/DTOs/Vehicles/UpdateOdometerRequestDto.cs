namespace DotCruz.Garage.Application.DTOs.Vehicles;

public sealed record UpdateOdometerRequestDto(
    int NewOdometer, 
    DateTimeOffset? RecordedAt = null
);
