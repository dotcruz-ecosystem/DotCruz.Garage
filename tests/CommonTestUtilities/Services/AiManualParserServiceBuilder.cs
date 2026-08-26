using DotCruz.Garage.Application.Abstractions.Services.Ai;
using DotCruz.Garage.Application.DTOs.Catalog;
using DotCruz.Garage.Domain.Enums;
using Moq;

namespace CommonTestUtilities.Services;

public class AiManualParserServiceBuilder
{
    private readonly Mock<IAiManualParserService> _mock = new();

    public Mock<IAiManualParserService> Mock => _mock;

    public AiManualParserServiceBuilder SetupExtractionResult(ExtractedManualResultDto result)
    {
        _mock.Setup(s => s.ExtractMaintenancePlanAsync(
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return this;
    }

    public AiManualParserServiceBuilder SetupDefaultExtraction(string brand = "Fiat", string model = "Strada", int year = 2023)
    {
        var defaultResult = new ExtractedManualResultDto(
            Brand: brand,
            Model: model,
            Year: year,
            Engine: "1.3",
            Version: "Endurance",
            Templates: new List<ExtractedMaintenancePlanItemDto>
            {
                new("Revisão 10k km", MaintenanceCategory.Engine, 10_000, 12, new List<string> { "Óleo", "Filtro" }),
                new("Revisão 20k km", MaintenanceCategory.Engine, 20_000, 24, new List<string> { "Óleo", "Filtro ar", "Filtro combustível" })
            }
        );

        return SetupExtractionResult(defaultResult);
    }

    public IAiManualParserService Build() => _mock.Object;
}
