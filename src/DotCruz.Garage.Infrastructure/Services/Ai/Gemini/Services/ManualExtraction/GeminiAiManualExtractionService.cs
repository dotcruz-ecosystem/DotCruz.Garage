using DotCruz.Garage.Application.Abstractions.Services.Ai;
using DotCruz.Garage.Application.DTOs.Ai;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.Enums;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.ResponseFormatsRequest;
using Microsoft.Extensions.Logging;

namespace DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Services.ManualExtraction;

public class GeminiAiManualExtractionService(IGeminiClient client, ILogger<GeminiAiManualExtractionService> logger) : IAiManualExtractionService
{
    public async Task<ExtractedManualResultDto> ExtractMaintenancePlanAsync(
        string brand, 
        string model, 
        string engine, 
        int year, 
        VehicleType type, 
        CancellationToken cancellationToken
    )
    {
        var vehicleTypeStr = type == VehicleType.Motorcycle ? "Motocicleta" : "Carro/Automóvel";
        var prompt = AiPrompts.BuildExtractMaintenancePlanPrompt(vehicleTypeStr, brand, model, engine, year);

        try
        {
            var checkpointItemSchema = ResponseFormatPropertyRequest.CreateObject("Marco gradual de revisão.")
                .AddProperty("odometer", ResponseFormatSchemaTypeEnum.Integer, "Quilometragem do marco inicial (ex: 1000, 5000, 10000)")
                .AddProperty("months", ResponseFormatSchemaTypeEnum.Integer, "Tempo acumulado em meses desde o veículo 0km (ex: 6, 12, 18)")
                .AddRequiredProperty(["odometer", "months"]);

            var checkpointsArraySchema = ResponseFormatPropertyRequest.CreateArray(checkpointItemSchema, "Lista de marcos graduais preliminares antes do regime estável.");

            var itemSchema = ResponseFormatPropertyRequest.CreateObject("Item de manutenção preventiva periódica.")
                .AddProperty("description", ResponseFormatSchemaTypeEnum.String, "Descrição atômica e individual do serviço (ex: Troca do óleo do motor; ou Troca do filtro de óleo; nunca agrupe itens)")
                .AddProperty("category", ResponseFormatSchemaTypeEnum.String, "Categoria do item: Engine, Brakes, Suspension, Tires, Transmission, Electrical ou General")
                .AddProperty("intervalOdometer", ResponseFormatSchemaTypeEnum.Integer, "Intervalo de quilometragem de regime contínuo estável (ex: 5000 ou 10000)")
                .AddProperty("intervalTimeMonths", ResponseFormatSchemaTypeEnum.Integer, "Intervalo em meses de regime contínuo estável (ex: 6 ou 12)")
                .AddProperty("isRequired", ResponseFormatSchemaTypeEnum.Boolean, "Indica se o item é obrigatório na revisão")
                .AddProperty("checkpoints", checkpointsArraySchema)
                .AddRequiredProperty(["description", "category", "intervalOdometer", "intervalTimeMonths"]);

            var result = await client.InitRequest("gemini-3.8-flash", prompt)
                .SetSearchTool()
                .SetPropertyToResponseFormat("brand", ResponseFormatSchemaTypeEnum.String, "Marca do veículo.")
                .SetPropertyToResponseFormat("model", ResponseFormatSchemaTypeEnum.String, "Modelo do veículo.")
                .SetPropertyToResponseFormat("engine", ResponseFormatSchemaTypeEnum.String, "Motorização do veículo (ex: 1.0, 2.0, 160cc, Elétrica, etc.).")
                .SetPropertyToResponseFormat("yearFrom", ResponseFormatSchemaTypeEnum.Integer, "Ano inicial da geração compatível com este manual.")
                .SetPropertyToResponseFormat("yearTo", ResponseFormatSchemaTypeEnum.Integer, "Ano final da geração compatível com este manual.")
                .SetPropertyToResponseFormat("sourceUrl", ResponseFormatSchemaTypeEnum.String, "URL oficial de referência ou do manual do proprietário encontrado na busca.")
                .SetArrayPropertyToResponseFormat("items", itemSchema, "Lista de itens de revisão periódica e manutenção preventiva.")
                .SetRequiredPropertyToResponseFormat(["brand", "model", "items"])
                .MakeRequest<GeminiManualExtractionResponse>(cancellationToken);

            if (result.Items == null || result.Items.Count == 0)
            {
                logger.LogWarning("Gemini retornou lista de itens vazia para {Brand} {Model}. Utilizando plano baseline.", brand, model);
                return GenerateBaselinePlan(brand, model, engine, year, type);
            }

            var items = result.Items.Select(i =>
            {
                var cat = ParseCategory(i.Category);
                var checkpoints = i.Checkpoints?
                    .Where(c => c.Odometer > 0)
                    .OrderBy(c => c.Odometer)
                    .Select(c => new ExtractedCheckpointDto(c.Odometer, c.Months))
                    .ToList() ?? new List<ExtractedCheckpointDto>();

                return new ExtractedMaintenanceItemDto(
                    i.Description ?? "Revisão Periódica",
                    cat,
                    i.IntervalOdometer,
                    i.IntervalTimeMonths,
                    i.IsRequired ?? true,
                    checkpoints);
            }).ToList();

            return new ExtractedManualResultDto(
                result.Brand ?? brand,
                result.Model ?? model,
                result.Engine ?? engine,
                result.YearFrom > 0 ? result.YearFrom : year,
                result.YearTo > 0 ? result.YearTo : year,
                result.SourceUrl,
                items);
        }
        catch (GeminiApiException ex)
        {
            logger.LogError(ex, "Falha ao extrair plano via Gemini AI para {Brand} {Model} {Engine} ({Year}). Status: {StatusCode}. Resposta: {ResponseContent}. Utilizando plano baseline.", brand, model, engine, year, ex.StatusCode, ex.ResponseContent);
            return GenerateBaselinePlan(brand, model, engine, year, type);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha inesperada ao extrair plano via Gemini AI para {Brand} {Model} {Engine} ({Year}). Utilizando plano baseline.", brand, model, engine, year);
            return GenerateBaselinePlan(brand, model, engine, year, type);
        }
    }

    private static MaintenanceCategory ParseCategory(string? category)
    {
        if (string.IsNullOrWhiteSpace(category)) return MaintenanceCategory.General;

        if (Enum.TryParse<MaintenanceCategory>(category, true, out var result))
            return result;

        var lower = category.ToLower();
        if (lower.Contains("óleo") || lower.Contains("oleo") || lower.Contains("motor") || lower.Contains("engine")) return MaintenanceCategory.Engine;
        if (lower.Contains("freio") || lower.Contains("brake") || lower.Contains("pastilha")) return MaintenanceCategory.Brakes;
        if (lower.Contains("pneu") || lower.Contains("tire") || lower.Contains("roda")) return MaintenanceCategory.Tires;
        if (lower.Contains("suspens") || lower.Contains("amortecedor")) return MaintenanceCategory.Suspension;
        if (lower.Contains("câmbio") || lower.Contains("cambio") || lower.Contains("transmiss") || lower.Contains("embreagem")) return MaintenanceCategory.Transmission;
        if (lower.Contains("elétric") || lower.Contains("eletric") || lower.Contains("bateria") || lower.Contains("vela")) return MaintenanceCategory.Electrical;

        return MaintenanceCategory.General;
    }

    private static ExtractedManualResultDto GenerateBaselinePlan(string brand, string model, string engine, int year, VehicleType type)
    {
        var items = new List<ExtractedMaintenanceItemDto>();

        if (type == VehicleType.Motorcycle)
        {
            items.Add(new ExtractedMaintenanceItemDto("Troca do Óleo do Motor", MaintenanceCategory.Engine, 3000, 6, true,
                new List<ExtractedCheckpointDto> { new(1000, 6) }));
            items.Add(new ExtractedMaintenanceItemDto("Troca do Filtro de Óleo", MaintenanceCategory.Engine, 6000, 6, true,
                new List<ExtractedCheckpointDto> { new(1000, 6) }));
            items.Add(new ExtractedMaintenanceItemDto("Ajuste e Lubrificação da Corrente/Transmissão", MaintenanceCategory.Transmission, 1000, 1, true));
            items.Add(new ExtractedMaintenanceItemDto("Inspeção e Troca de Pastilhas/Lonas de Freio", MaintenanceCategory.Brakes, 6000, 6, true));
            items.Add(new ExtractedMaintenanceItemDto("Troca do Fluido de Freio", MaintenanceCategory.Brakes, 12000, 24, true));
            items.Add(new ExtractedMaintenanceItemDto("Troca da Vela de Ignição", MaintenanceCategory.Electrical, 12000, 12, true));
            items.Add(new ExtractedMaintenanceItemDto("Troca do Filtro de Ar", MaintenanceCategory.Engine, 12000, 12, true));
        }
        else
        {
            items.Add(new ExtractedMaintenanceItemDto("Troca do Óleo do Motor", MaintenanceCategory.Engine, 10000, 12, true));
            items.Add(new ExtractedMaintenanceItemDto("Troca do Filtro de Óleo", MaintenanceCategory.Engine, 10000, 12, true));
            items.Add(new ExtractedMaintenanceItemDto("Troca do Filtro de Combustível", MaintenanceCategory.Engine, 10000, 12, true));
            items.Add(new ExtractedMaintenanceItemDto("Troca do Filtro de Ar do Motor", MaintenanceCategory.Engine, 20000, 24, true));
            items.Add(new ExtractedMaintenanceItemDto("Troca do Filtro de Cabine (Ar Condicionado)", MaintenanceCategory.General, 10000, 12, true));
            items.Add(new ExtractedMaintenanceItemDto("Troca do Fluido de Freio", MaintenanceCategory.Brakes, 40000, 24, true));
            items.Add(new ExtractedMaintenanceItemDto("Inspeção de Pastilhas e Discos de Freio", MaintenanceCategory.Brakes, 10000, 12, true));
            items.Add(new ExtractedMaintenanceItemDto("Alinhamento e Balanceamento dos Pneus", MaintenanceCategory.Tires, 10000, 12, true));
            items.Add(new ExtractedMaintenanceItemDto("Troca das Velas de Ignição", MaintenanceCategory.Electrical, 40000, 36, true));
            items.Add(new ExtractedMaintenanceItemDto("Troca da Correia Dentada / Acessórios", MaintenanceCategory.Engine, 60000, 48, true));
            items.Add(new ExtractedMaintenanceItemDto("Troca do Líquido de Arrefecimento", MaintenanceCategory.Engine, 60000, 48, true));
        }

        return new ExtractedManualResultDto(
            brand,
            model,
            engine,
            year,
            year,
            "https://www.google.com/search?q=manual+proprietario+" + Uri.EscapeDataString($"{brand} {model} {engine}"),
            items);
    }
}
