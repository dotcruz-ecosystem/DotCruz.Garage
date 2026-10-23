using CommonTestUtilities.Services;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Services.ManualExtraction;
using System.Net;

namespace Infrastructure.Test.Services;

public class GeminiAiManualExtractionServiceTest
{
    [Fact]
    public async Task Success_WhenGeminiReturnsValidPlan()
    {
        // Arrange
        var geminiResponse = new GeminiManualExtractionResponse
        {
            Brand = "Honda",
            Model = "Civic",
            Engine = "2.0",
            YearFrom = 2020,
            YearTo = 2024,
            SourceUrl = "https://honda.com.br/manual",
            Items =
            [
                new()
                {
                    Description = "Troca de óleo do motor",
                    Category = "Engine",
                    IntervalOdometer = 10000,
                    IntervalTimeMonths = 12,
                    IsRequired = true
                }
            ]
        };

        var client = new GeminiClientBuilder()
            .SetupMakeRequest(geminiResponse)
            .Build();

        var logger = LoggerBuilder.Build<GeminiAiManualExtractionService>();

        var service = new GeminiAiManualExtractionService(client, logger);

        // Act
        var result = await service.ExtractMaintenancePlanAsync("Honda", "Civic", "2.0", 2022, VehicleType.Car, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Honda", result.Brand);
        Assert.Equal("Civic", result.Model);
        Assert.Equal("2.0", result.Engine);
        Assert.Equal("https://honda.com.br/manual", result.SourceUrl);
        Assert.Single(result.Items);
        var item = result.Items.First();
        Assert.Equal("Troca de óleo do motor", item.Description);
        Assert.Equal(MaintenanceCategory.Engine, item.Category);
        Assert.Equal(10000, item.IntervalOdometer);
        Assert.Equal(12, item.IntervalTimeMonths);
    }

    [Fact]
    public async Task Fallback_WhenGeminiThrowsException()
    {
        // Arrange
        var client = new GeminiClientBuilder()
            .SetupMakeRequestThrows(new GeminiApiException(HttpStatusCode.TooManyRequests, "Quota Exceeded"))
            .Build();

        var logger = LoggerBuilder.Build<GeminiAiManualExtractionService>();

        var service = new GeminiAiManualExtractionService(client, logger);

        // Act
        var result = await service.ExtractMaintenancePlanAsync("Honda", "Civic", "2.0", 2022, VehicleType.Car, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Honda", result.Brand);
        Assert.Equal("Civic", result.Model);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task Fallback_WhenGeminiReturnsEmptyItems()
    {
        // Arrange
        var geminiResponse = new GeminiManualExtractionResponse
        {
            Brand = "Honda",
            Model = "Civic",
            Items = []
        };

        var client = new GeminiClientBuilder()
            .SetupMakeRequest(geminiResponse)
            .Build();

        var logger = LoggerBuilder.Build<GeminiAiManualExtractionService>();

        var service = new GeminiAiManualExtractionService(client, logger);

        // Act
        var result = await service.ExtractMaintenancePlanAsync("Honda", "Civic", "2.0", 2022, VehicleType.Car, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Honda", result.Brand);
        Assert.NotEmpty(result.Items);
    }
}
