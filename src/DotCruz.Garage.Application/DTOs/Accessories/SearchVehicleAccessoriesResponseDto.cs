namespace DotCruz.Garage.Application.DTOs.Accessories;

public sealed record SearchVehicleAccessoriesResponseDto(
    IReadOnlyCollection<VehicleAccessoryDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount
);
