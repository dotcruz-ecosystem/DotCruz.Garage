using DotCruz.Garage.Application.Abstractions.Services.Ai;
using DotCruz.Garage.Application.DTOs.ServiceOrders;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Enums;
using Moq;

namespace CommonTestUtilities.Services;

public class AiServiceOrderExtractionServiceBuilder
{
    private readonly Mock<IAiServiceOrderExtractionService> _mock = new();
    private bool _isConfigured;

    public Mock<IAiServiceOrderExtractionService> Mock => _mock;

    public AiServiceOrderExtractionServiceBuilder SetupExtractServiceOrder(ExtractedServiceOrderDto result)
    {
        _isConfigured = true;
        _mock.Setup(s => s.ExtractServiceOrderAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IReadOnlyCollection<MaintenanceSchedule>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return this;
    }

    public AiServiceOrderExtractionServiceBuilder SetupThrows(Exception exception)
    {
        _isConfigured = true;
        _mock.Setup(s => s.ExtractServiceOrderAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IReadOnlyCollection<MaintenanceSchedule>>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        return this;
    }

    public IAiServiceOrderExtractionService Build()
    {
        if (!_isConfigured)
        {
            _mock.Setup(s => s.ExtractServiceOrderAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IReadOnlyCollection<MaintenanceSchedule>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((string fileUrl, string fileName, string contentType, IReadOnlyCollection<MaintenanceSchedule> activeSchedules, CancellationToken _) =>
                {
                    return new ExtractedServiceOrderDto(
                        OrderNumber: "39196",
                        WorkshopName: "Moto Selvagem",
                        PerformedAt: DateTimeOffset.UtcNow,
                        OdometerAtService: 24891,
                        TotalCost: 178.70m,
                        Discount: 0m,
                        Notes: "Pedal de freio caído",
                        DetectedVehiclePlate: "TAL-1G31",
                        DetectedVehicleModel: "MT 03",
                        Items: new List<ExtractedServiceOrderItemDto>
                        {
                            new("PARAFUSO PEDAL CAMBIO", ServiceOrderItemType.Part, MaintenanceCategory.General, 1, 13.70m, 13.70m, "136592MGL"),
                            new("TROCA VELA IGNICAO", ServiceOrderItemType.Labor, MaintenanceCategory.Engine, 1, 100m, 100m, "136164", activeSchedules.FirstOrDefault()?.Id)
                        },
                        Warnings: new List<string>());
                });
        }

        return _mock.Object;
    }
}
