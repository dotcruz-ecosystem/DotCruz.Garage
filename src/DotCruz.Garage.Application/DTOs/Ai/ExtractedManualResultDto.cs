namespace DotCruz.Garage.Application.DTOs.Ai;

public record ExtractedManualResultDto(
    string Brand,
    string Model,
    string Engine,
    int YearFrom,
    int YearTo,
    string? SourceUrl,
    IReadOnlyCollection<ExtractedMaintenanceItemDto> Items
);
