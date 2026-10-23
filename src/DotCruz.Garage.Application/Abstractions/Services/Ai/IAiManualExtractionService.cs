using DotCruz.Garage.Application.DTOs.Ai;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Application.Abstractions.Services.Ai;

public interface IAiManualExtractionService
{
    Task<ExtractedManualResultDto> ExtractMaintenancePlanAsync(
        string brand,
        string model,
        string engine,
        int year,
        VehicleType type,
        CancellationToken cancellationToken);
}
