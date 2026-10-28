using System.Text.Json.Serialization;

namespace DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Services.ServiceOrderExtraction;

public class GeminiExtractedServiceOrderResponse
{
    [JsonPropertyName("orderNumber")]
    public string? OrderNumber { get; set; }

    [JsonPropertyName("workshopName")]
    public string? WorkshopName { get; set; }

    [JsonPropertyName("performedAt")]
    public string? PerformedAt { get; set; }

    [JsonPropertyName("odometerAtService")]
    public int? OdometerAtService { get; set; }

    [JsonPropertyName("totalCost")]
    public decimal? TotalCost { get; set; }

    [JsonPropertyName("discount")]
    public decimal? Discount { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("detectedVehiclePlate")]
    public string? DetectedVehiclePlate { get; set; }

    [JsonPropertyName("detectedVehicleModel")]
    public string? DetectedVehicleModel { get; set; }

    [JsonPropertyName("items")]
    public List<GeminiExtractedItemResponse>? Items { get; set; }
}

public class GeminiExtractedItemResponse
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("quantity")]
    public decimal? Quantity { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal? UnitPrice { get; set; }

    [JsonPropertyName("totalCost")]
    public decimal? TotalCost { get; set; }

    [JsonPropertyName("partNumber")]
    public string? PartNumber { get; set; }

    [JsonPropertyName("matchedScheduleId")]
    public string? MatchedScheduleId { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}
