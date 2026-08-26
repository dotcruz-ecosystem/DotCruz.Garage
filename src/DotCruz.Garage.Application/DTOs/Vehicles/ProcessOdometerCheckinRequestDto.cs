namespace DotCruz.Garage.Application.DTOs.Vehicles;

public record ProcessOdometerCheckinRequestDto(
    int NewOdometer,
    DateTimeOffset? RecordedAt = null);
