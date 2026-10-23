namespace DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Services.ManualExtraction;

public class GeminiManualExtractionResponse
{
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? Engine { get; set; }
    public int YearFrom { get; set; }
    public int YearTo { get; set; }
    public string? SourceUrl { get; set; }
    public List<GeminiManualExtractionItemResponse>? Items { get; set; }
}

public class GeminiManualExtractionItemResponse
{
    public string? Description { get; set; }
    public string? Category { get; set; }
    public int? IntervalOdometer { get; set; }
    public int? IntervalTimeMonths { get; set; }
    public bool? IsRequired { get; set; }
}
