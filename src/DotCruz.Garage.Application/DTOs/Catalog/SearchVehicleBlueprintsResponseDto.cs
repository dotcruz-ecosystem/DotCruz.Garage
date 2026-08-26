namespace DotCruz.Garage.Application.DTOs.Catalog;

public record SearchVehicleBlueprintsResponseDto(
    IReadOnlyCollection<VehicleBlueprintDto> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);
