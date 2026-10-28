using DotCruz.Garage.Application.Abstractions.Services.Ai;
using DotCruz.Garage.Application.DTOs.Ai;
using DotCruz.Garage.Domain.Enums;
using Moq;

namespace CommonTestUtilities.Services;

public class AiManualExtractionServiceBuilder
{
    private readonly Mock<IAiManualExtractionService> _mock = new();

    private bool _isConfigured;

    public Mock<IAiManualExtractionService> Mock => _mock;

    public AiManualExtractionServiceBuilder SetupExtractMaintenancePlan(ExtractedManualResultDto result)
    {
        _isConfigured = true;
        _mock.Setup(s => s.ExtractMaintenancePlanAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<VehicleType>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return this;
    }

    public AiManualExtractionServiceBuilder SetupThrows(Exception exception)
    {
        _isConfigured = true;
        _mock.Setup(s => s.ExtractMaintenancePlanAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<VehicleType>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        return this;
    }

    public IAiManualExtractionService Build()
    {
        if (!_isConfigured)
        {
            _mock.Setup(s => s.ExtractMaintenancePlanAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<VehicleType>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((string brand, string model, string engine, int year, VehicleType type, CancellationToken _) =>
                {
                    var items = new List<ExtractedMaintenanceItemDto>
                    {
                        new("Troca de Óleo do Motor", MaintenanceCategory.Engine, 10000, 12, true),
                        new("Troca do Filtro de Combustível", MaintenanceCategory.Engine, 10000, 12, true)
                    };

                    return new ExtractedManualResultDto(
                        brand,
                        model,
                        engine,
                        year,
                        year + 3,
                        "https://manuals.example.com/test",
                        items);
                });
        }

        return _mock.Object;
    }
}
