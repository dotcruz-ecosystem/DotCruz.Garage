namespace DotCruz.Garage.Application.DTOs.Vehicles;

public record GenerateOdometerCheckinTokenResponseDto(
    Guid VehicleId,
    string Token,
    string MagicLinkUrl,
    DateTimeOffset ExpiresAt);
