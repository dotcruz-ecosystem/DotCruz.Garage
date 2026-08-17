namespace DotCruz.Garage.Application.DTOs.Vehicles;

public sealed record SearchVehiclesResponseDto(
    IReadOnlyCollection<VehicleDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount
);
