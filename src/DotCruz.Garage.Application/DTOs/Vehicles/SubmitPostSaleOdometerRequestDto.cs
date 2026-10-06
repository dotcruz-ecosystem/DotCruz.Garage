namespace DotCruz.Garage.Application.DTOs.Vehicles;

public sealed record SubmitPostSaleOdometerRequestDto(int NewOdometer, DateTimeOffset? RecordedAt = null);
