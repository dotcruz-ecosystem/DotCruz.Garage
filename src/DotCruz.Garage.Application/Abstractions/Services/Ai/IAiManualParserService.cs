using DotCruz.Garage.Application.DTOs.Catalog;

namespace DotCruz.Garage.Application.Abstractions.Services.Ai;

public interface IAiManualParserService
{
    Task<ExtractedManualResultDto> ExtractMaintenancePlanAsync(
        Stream fileStream,
        string fileName,
        string brand,
        string model,
        int year,
        string? engine,
        CancellationToken cancellationToken);
}
