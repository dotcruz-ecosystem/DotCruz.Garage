namespace DotCruz.Garage.Application.DTOs.Fines;

public record IndicateFineDriverRequestDto(
    Guid DriverId,
    string? Protocol = null);
