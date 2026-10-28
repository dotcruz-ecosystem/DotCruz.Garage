using DotCruz.Garage.Application.DTOs.ServiceOrders;
using DotCruz.Garage.Domain.Entities.Maintenance;

namespace DotCruz.Garage.Application.Abstractions.Services.Ai;

public interface IAiServiceOrderExtractionService
{
    Task<ExtractedServiceOrderDto> ExtractServiceOrderAsync(
        string fileUrl,
        string fileName,
        string contentType,
        IReadOnlyCollection<MaintenanceSchedule> activeSchedules,
        CancellationToken cancellationToken);
}
