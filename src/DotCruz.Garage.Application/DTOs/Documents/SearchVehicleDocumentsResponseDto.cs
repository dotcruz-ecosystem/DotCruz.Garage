namespace DotCruz.Garage.Application.DTOs.Documents;

public sealed record SearchVehicleDocumentsResponseDto(
    IReadOnlyCollection<VehicleDocumentDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount
);
