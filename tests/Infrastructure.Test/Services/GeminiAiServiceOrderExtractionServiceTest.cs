using CommonTestUtilities.Entities;
using CommonTestUtilities.Services;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Services.ServiceOrderExtraction;
using System.Net;

namespace Infrastructure.Test.Services;

public class GeminiAiServiceOrderExtractionServiceTest
{
    [Fact]
    public async Task Success_WhenGeminiReturnsValidServiceOrder()
    {
        // Arrange
        var geminiResponse = new GeminiExtractedServiceOrderResponse
        {
            OrderNumber = "9252",
            WorkshopName = "Oficina Tio Dani",
            PerformedAt = "2026-03-11",
            OdometerAtService = 237971,
            TotalCost = 864.00m,
            Discount = 0,
            Notes = "Defeito: mangueiras furadas",
            DetectedVehiclePlate = "MNG-8655",
            DetectedVehicleModel = "CORSA 95 1.4",
            Items =
            [
                new()
                {
                    Description = "MANGUEIRAS DO RADIADOR PAR",
                    Type = "Part",
                    Category = "Engine",
                    Quantity = 1,
                    UnitPrice = 98.00m,
                    TotalCost = 98.00m
                },
                new()
                {
                    Description = "SERVIÇOS",
                    Type = "Labor",
                    Category = "General",
                    Quantity = 1,
                    UnitPrice = 270.00m,
                    TotalCost = 270.00m
                }
            ]
        };

        var client = new GeminiClientBuilder()
            .SetupMakeRequest(geminiResponse)
            .Build();

        var httpClient = new HttpClientBuilder()
            .SetupGetByteArray([1, 2, 3, 4])
            .Build();

        var logger = LoggerBuilder.Build<GeminiAiServiceOrderExtractionService>();

        var service = new GeminiAiServiceOrderExtractionService(client, httpClient, logger);

        var schedule = MaintenanceScheduleBuilder.Build();

        // Act
        var result = await service.ExtractServiceOrderAsync(
            "https://r2.dotcruz.com/os-corsa.jpg",
            "os-corsa.jpg",
            "image/jpeg",
            new[] { schedule },
            TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("9252", result.OrderNumber);
        Assert.Equal("Oficina Tio Dani", result.WorkshopName);
        Assert.Equal(237971, result.OdometerAtService);
        Assert.Equal(864.00m, result.TotalCost);
        Assert.Equal("MNG-8655", result.DetectedVehiclePlate);
        Assert.Equal("CORSA 95 1.4", result.DetectedVehicleModel);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(ServiceOrderItemType.Part, result.Items[0].Type);
        Assert.Equal(MaintenanceCategory.Engine, result.Items[0].Category);
        Assert.Equal(ServiceOrderItemType.Labor, result.Items[1].Type);
    }

    [Fact]
    public async Task Throws_GeminiApiException_When_DownloadFails()
    {
        // Arrange
        var client = new GeminiClientBuilder().Build();
        var httpClient = new HttpClientBuilder()
            .SetupGetByteArray([], HttpStatusCode.NotFound)
            .Build();
        var logger = LoggerBuilder.Build<GeminiAiServiceOrderExtractionService>();

        var service = new GeminiAiServiceOrderExtractionService(client, httpClient, logger);

        // Act & Assert
        await Assert.ThrowsAsync<GeminiApiException>(() =>
            service.ExtractServiceOrderAsync(
                "https://r2.dotcruz.com/inexistente.jpg",
                "inexistente.jpg",
                "image/jpeg",
                [],
                TestContext.Current.CancellationToken));
    }
}
