namespace DotCruz.Garage.Application.DTOs.Fines;

public sealed record SearchTrafficFinesResponseDto(
    IReadOnlyCollection<TrafficFineDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount
);
