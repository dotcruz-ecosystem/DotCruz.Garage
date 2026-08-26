using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotCruz.Garage.Application.Abstractions.Services.Ai;
using DotCruz.Garage.Application.DTOs.Catalog;
using DotCruz.Garage.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DotCruz.Garage.Infrastructure.Services.Ai;

public class GeminiAiManualParserService : IAiManualParserService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeminiAiManualParserService> _logger;

    public GeminiAiManualParserService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GeminiAiManualParserService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ExtractedManualResultDto> ExtractMaintenancePlanAsync(
        Stream fileStream,
        string fileName,
        string brand,
        string model,
        int year,
        string? engine,
        CancellationToken cancellationToken)
    {
        var apiKey = _configuration["Settings:GeminiApiKey"] 
                     ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("Gemini API key not configured. Generating standard maintenance blueprint for {Brand} {Model} {Year}.", brand, model, year);
            return GenerateFallbackMaintenancePlan(brand, model, year, engine);
        }

        try
        {
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();
            var base64Data = Convert.ToBase64String(fileBytes);

            var modelName = _configuration["Settings:GeminiModel"] ?? "gemini-2.5-flash";
            var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent?key={apiKey}";

            var promptText = $$"""
                You are an expert automotive technician.
                Analyze this owner manual PDF for vehicle:
                Brand: {{brand}}
                Model: {{model}}
                Year: {{year}}
                Engine: {{engine ?? "Default"}}

                Extract the official periodic maintenance schedule (revisões periódicas).
                Return a strict JSON object with this exact structure:
                {
                  "brand": "{{brand}}",
                  "model": "{{model}}",
                  "year": {{year}},
                  "engine": "{{engine}}",
                  "version": "Standard",
                  "templates": [
                    {
                      "description": "Revisão de 10.000 km ou 12 meses",
                      "category": "Engine",
                      "intervalOdometer": 10000,
                      "intervalTimeMonths": 12,
                      "items": ["Troca de óleo do motor", "Troca do filtro de óleo", "Inspeção de freios e suspensão"],
                      "isRecurring": true
                    }
                  ]
                }

                Category must be one of: Engine, Brakes, Tires, Suspension, Transmission, Electrical, General.
                Return ONLY the JSON. No markdown code blocks.
                """;

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = promptText },
                            new
                            {
                                inlineData = new
                                {
                                    mimeType = "application/pdf",
                                    data = base64Data
                                }
                            }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.1,
                    responseMimeType = "application/json"
                }
            };

            var response = await _httpClient.PostAsJsonAsync(requestUrl, payload, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Gemini API call failed with status {StatusCode}: {Error}", response.StatusCode, errorBody);
                return GenerateFallbackMaintenancePlan(brand, model, year, engine);
            }

            var resultJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var parsed = ParseGeminiResponse(resultJson, brand, model, year, engine);
            return parsed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during Gemini AI manual extraction for {Brand} {Model}. Falling back to default plan.", brand, model);
            return GenerateFallbackMaintenancePlan(brand, model, year, engine);
        }
    }

    private ExtractedManualResultDto ParseGeminiResponse(
        string rawJson,
        string brand,
        string model,
        int year,
        string? engine)
    {
        try
        {
            using var doc = JsonDocument.Parse(rawJson);
            var root = doc.RootElement;
            var text = root
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrWhiteSpace(text))
            {
                return GenerateFallbackMaintenancePlan(brand, model, year, engine);
            }

            var cleanJson = text.Trim();
            if (cleanJson.StartsWith("```json"))
            {
                cleanJson = cleanJson.Substring(7);
            }
            if (cleanJson.StartsWith("```"))
            {
                cleanJson = cleanJson.Substring(3);
            }
            if (cleanJson.EndsWith("```"))
            {
                cleanJson = cleanJson.Substring(0, cleanJson.Length - 3);
            }
            cleanJson = cleanJson.Trim();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            var extracted = JsonSerializer.Deserialize<ExtractedManualJsonModel>(cleanJson, options);
            if (extracted == null || extracted.Templates == null || extracted.Templates.Count == 0)
            {
                return GenerateFallbackMaintenancePlan(brand, model, year, engine);
            }

            return new ExtractedManualResultDto(
                Brand: string.IsNullOrWhiteSpace(extracted.Brand) ? brand : extracted.Brand,
                Model: string.IsNullOrWhiteSpace(extracted.Model) ? model : extracted.Model,
                Year: extracted.Year > 0 ? extracted.Year : year,
                Engine: string.IsNullOrWhiteSpace(extracted.Engine) ? engine : extracted.Engine,
                Version: extracted.Version,
                Templates: extracted.Templates.Select(t => new ExtractedMaintenancePlanItemDto(
                    t.Description ?? $"Revisão {t.IntervalOdometer} km",
                    Enum.TryParse<MaintenanceCategory>(t.Category, true, out var cat) ? cat : MaintenanceCategory.General,
                    t.IntervalOdometer,
                    t.IntervalTimeMonths,
                    t.Items ?? new List<string>(),
                    t.IsRecurring
                )).ToList()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse Gemini JSON output. Using fallback.");
            return GenerateFallbackMaintenancePlan(brand, model, year, engine);
        }
    }

    public static ExtractedManualResultDto GenerateFallbackMaintenancePlan(
        string brand,
        string model,
        int year,
        string? engine)
    {
        return new ExtractedManualResultDto(
            Brand: brand,
            Model: model,
            Year: year,
            Engine: engine,
            Version: "Padrão",
            Templates: new List<ExtractedMaintenancePlanItemDto>
            {
                new(
                    Description: "Revisão 10.000 km / 12 meses",
                    Category: MaintenanceCategory.Engine,
                    IntervalOdometer: 10_000,
                    IntervalTimeMonths: 12,
                    Items: new List<string> { "Troca de óleo do motor", "Troca do filtro de óleo", "Inspeção geral e freios" },
                    IsRecurring: true
                ),
                new(
                    Description: "Revisão 20.000 km / 24 meses",
                    Category: MaintenanceCategory.Engine,
                    IntervalOdometer: 20_000,
                    IntervalTimeMonths: 24,
                    Items: new List<string> { "Troca de óleo", "Filtro de ar do motor", "Filtro de combustível", "Filtro de cabine" },
                    IsRecurring: true
                ),
                new(
                    Description: "Revisão 30.000 km / 36 meses",
                    Category: MaintenanceCategory.Brakes,
                    IntervalOdometer: 30_000,
                    IntervalTimeMonths: 36,
                    Items: new List<string> { "Troca de óleo e filtro", "Substituição do fluido de freio", "Alinhamento e balanceamento" },
                    IsRecurring: true
                ),
                new(
                    Description: "Revisão 40.000 km / 48 meses",
                    Category: MaintenanceCategory.Engine,
                    IntervalOdometer: 40_000,
                    IntervalTimeMonths: 48,
                    Items: new List<string> { "Troca de velas de ignição", "Troca de óleo e todos os filtros", "Inspeção de correias" },
                    IsRecurring: true
                ),
                new(
                    Description: "Revisão 50.000 km / 60 meses",
                    Category: MaintenanceCategory.Transmission,
                    IntervalOdometer: 50_000,
                    IntervalTimeMonths: 60,
                    Items: new List<string> { "Troca de óleo do câmbio / transmissão", "Troca do líquido de arrefecimento", "Inspeção de suspensão" },
                    IsRecurring: true
                ),
                new(
                    Description: "Revisão 60.000 km / 72 meses",
                    Category: MaintenanceCategory.Engine,
                    IntervalOdometer: 60_000,
                    IntervalTimeMonths: 72,
                    Items: new List<string> { "Substituição da correia dentada / acessórios", "Troca de óleo e filtros", "Inspeção geral" },
                    IsRecurring: true
                )
            }
        );
    }

    private class ExtractedManualJsonModel
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }
        public string? Engine { get; set; }
        public string? Version { get; set; }
        public List<ExtractedTemplateJsonModel>? Templates { get; set; }
    }

    private class ExtractedTemplateJsonModel
    {
        public string? Description { get; set; }
        public string? Category { get; set; }
        public int? IntervalOdometer { get; set; }
        public int? IntervalTimeMonths { get; set; }
        public List<string>? Items { get; set; }
        public bool IsRecurring { get; set; } = true;
    }
}
