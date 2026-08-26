namespace DotCruz.Garage.Application.DTOs.Drivers;

public sealed record SearchDriversResponseDto(
    IReadOnlyCollection<DriverDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount
);
