using DotCruz.Garage.Application.Abstractions.Services.Ai;
using DotCruz.Garage.Application.DTOs.ServiceOrders;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.Enums;
using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.ResponseFormatsRequest;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Services.ServiceOrderExtraction;

public class GeminiAiServiceOrderExtractionService : IAiServiceOrderExtractionService
{
    private readonly IGeminiClient _client;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeminiAiServiceOrderExtractionService> _logger;

    public GeminiAiServiceOrderExtractionService(
        IGeminiClient client,
        HttpClient httpClient,
        ILogger<GeminiAiServiceOrderExtractionService> logger)
    {
        _client = client;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ExtractedServiceOrderDto> ExtractServiceOrderAsync(
        string fileUrl,
        string fileName,
        string contentType,
        IReadOnlyCollection<MaintenanceSchedule> activeSchedules,
        CancellationToken cancellationToken)
    {
        var warnings = new List<string>();

        byte[] fileBytes;
        try
        {
            fileBytes = await _httpClient.GetByteArrayAsync(fileUrl, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao baixar arquivo para extração de OS a partir da URL {FileUrl}", fileUrl);
            throw new GeminiApiException(System.Net.HttpStatusCode.BadRequest, "Não foi possível baixar o documento anexo para análise da IA.");
        }

        var prompt = AiPrompts.BuildExtractServiceOrderPrompt(activeSchedules);

        try
        {
            var itemSchema = ResponseFormatPropertyRequest.CreateObject("Item discriminado (peça ou serviço) da ordem de serviço.")
                .AddProperty("description", ResponseFormatSchemaTypeEnum.String, "Descrição do item conforme documento.")
                .AddProperty("type", ResponseFormatSchemaTypeEnum.String, "Tipo: 'Part' para peça/produto ou 'Labor' para mão de obra/serviço.")
                .AddProperty("category", ResponseFormatSchemaTypeEnum.String, "Categoria: Engine, Brakes, Suspension, Tires, Transmission, Electrical ou General.")
                .AddProperty("quantity", ResponseFormatSchemaTypeEnum.Number, "Quantidade do item.")
                .AddProperty("unitPrice", ResponseFormatSchemaTypeEnum.Number, "Valor unitário em R$.")
                .AddProperty("totalCost", ResponseFormatSchemaTypeEnum.Number, "Valor total do item em R$.")
                .AddProperty("partNumber", ResponseFormatSchemaTypeEnum.String, "Código de catálogo ou referência da peça se existir.")
                .AddProperty("matchedScheduleId", ResponseFormatSchemaTypeEnum.String, "ID em formato GUID do plano preventivo correspondente, ou nulo.")
                .AddProperty("notes", ResponseFormatSchemaTypeEnum.String, "Observações específicas do item.")
                .AddRequiredProperty(["description", "type", "totalCost"]);

            var itemsArraySchema = ResponseFormatPropertyRequest.CreateArray(itemSchema, "Lista de peças e serviços discriminados.");

            var result = await _client.InitMultimodalRequest("gemini-3.8-flash", prompt, fileBytes, contentType)
                .SetPropertyToResponseFormat("orderNumber", ResponseFormatSchemaTypeEnum.String, "Número da ordem de serviço ou nota fiscal.")
                .SetPropertyToResponseFormat("workshopName", ResponseFormatSchemaTypeEnum.String, "Nome fantasia da oficina ou prestador.")
                .SetPropertyToResponseFormat("performedAt", ResponseFormatSchemaTypeEnum.String, "Data do serviço no formato ISO AAAA-MM-DD.")
                .SetPropertyToResponseFormat("odometerAtService", ResponseFormatSchemaTypeEnum.Integer, "Quilometragem (KM) do odômetro registrada na OS.")
                .SetPropertyToResponseFormat("totalCost", ResponseFormatSchemaTypeEnum.Number, "Valor total final da OS / nota fiscal.")
                .SetPropertyToResponseFormat("discount", ResponseFormatSchemaTypeEnum.Number, "Valor do desconto concedido em R$.")
                .SetPropertyToResponseFormat("notes", ResponseFormatSchemaTypeEnum.String, "Observações gerais ou reclamações relatadas.")
                .SetPropertyToResponseFormat("detectedVehiclePlate", ResponseFormatSchemaTypeEnum.String, "Placa do veículo identificada no documento.")
                .SetPropertyToResponseFormat("detectedVehicleModel", ResponseFormatSchemaTypeEnum.String, "Modelo ou descrição do veículo identificada.")
                .SetArrayPropertyToResponseFormat("items", itemSchema, "Lista de peças e serviços da OS.")
                .MakeRequest<GeminiExtractedServiceOrderResponse>(cancellationToken);

            var activeScheduleIds = activeSchedules.Select(s => s.Id).ToHashSet();

            var mappedItems = (result.Items ?? []).Select(i =>
            {
                var type = ParseItemType(i.Type);
                var category = ParseCategory(i.Category);
                var quantity = i.Quantity.HasValue && i.Quantity.Value > 0 ? i.Quantity.Value : 1;
                var totalCost = i.TotalCost.HasValue && i.TotalCost.Value >= 0 ? i.TotalCost.Value : 0;
                var unitPrice = i.UnitPrice.HasValue && i.UnitPrice.Value >= 0 ? i.UnitPrice.Value : (quantity > 0 ? totalCost / quantity : totalCost);

                Guid? matchedScheduleId = null;
                if (!string.IsNullOrWhiteSpace(i.MatchedScheduleId) &&
                    Guid.TryParse(i.MatchedScheduleId, out var parsedGuid) &&
                    activeScheduleIds.Contains(parsedGuid))
                {
                    matchedScheduleId = parsedGuid;
                }

                return new ExtractedServiceOrderItemDto(
                    Description: string.IsNullOrWhiteSpace(i.Description) ? "Item sem descrição" : i.Description.Trim(),
                    Type: type,
                    Category: category,
                    Quantity: quantity,
                    UnitPrice: Math.Round(unitPrice, 2),
                    TotalCost: Math.Round(totalCost, 2),
                    PartNumber: string.IsNullOrWhiteSpace(i.PartNumber) ? null : i.PartNumber.Trim(),
                    MatchedMaintenanceScheduleId: matchedScheduleId,
                    Notes: string.IsNullOrWhiteSpace(i.Notes) ? null : i.Notes.Trim());
            }).ToList();

            if (mappedItems.Count == 0)
            {
                warnings.Add("Nenhum item discriminado foi identificado com certeza no documento.");
            }

            DateTimeOffset? performedAt = null;
            if (!string.IsNullOrWhiteSpace(result.PerformedAt))
            {
                if (DateTimeOffset.TryParse(result.PerformedAt, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    performedAt = parsedDate;
                }
                else if (DateTime.TryParse(result.PerformedAt, new CultureInfo("pt-BR"), DateTimeStyles.None, out var brDate))
                {
                    performedAt = new DateTimeOffset(brDate, TimeSpan.Zero);
                }
            }

            if (!result.OdometerAtService.HasValue)
            {
                warnings.Add("Quilometragem (KM) não identificada no documento.");
            }

            var calculatedTotal = mappedItems.Sum(it => it.TotalCost);
            var totalCost = result.TotalCost.HasValue && result.TotalCost.Value > 0
                ? result.TotalCost.Value
                : calculatedTotal;

            var discount = result.Discount.HasValue && result.Discount.Value >= 0 ? result.Discount.Value : 0;

            return new ExtractedServiceOrderDto(
                OrderNumber: string.IsNullOrWhiteSpace(result.OrderNumber) ? null : result.OrderNumber.Trim(),
                WorkshopName: string.IsNullOrWhiteSpace(result.WorkshopName) ? null : result.WorkshopName.Trim(),
                PerformedAt: performedAt,
                OdometerAtService: result.OdometerAtService,
                TotalCost: Math.Round(totalCost, 2),
                Discount: Math.Round(discount, 2),
                Notes: string.IsNullOrWhiteSpace(result.Notes) ? null : result.Notes.Trim(),
                DetectedVehiclePlate: string.IsNullOrWhiteSpace(result.DetectedVehiclePlate) ? null : result.DetectedVehiclePlate.Trim().ToUpperInvariant(),
                DetectedVehicleModel: string.IsNullOrWhiteSpace(result.DetectedVehicleModel) ? null : result.DetectedVehicleModel.Trim(),
                Items: mappedItems,
                Warnings: warnings);
        }
        catch (GeminiApiException ex)
        {
            _logger.LogError(ex, "Falha na chamada ao Gemini AI para extração de OS a partir de {FileName}. Status: {StatusCode}", fileName, ex.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao processar extração de OS por IA do arquivo {FileName}", fileName);
            throw new GeminiApiException(System.Net.HttpStatusCode.InternalServerError, $"Erro ao processar documento com IA: {ex.Message}");
        }
    }

    private static ServiceOrderItemType ParseItemType(string? type)
    {
        if (string.IsNullOrWhiteSpace(type)) return ServiceOrderItemType.Part;

        var lower = type.Trim().ToLowerInvariant();
        if (lower.Contains("labor") || lower.Contains("serviço") || lower.Contains("servico") || lower.Contains("mão de obra") || lower.Contains("mao de obra"))
        {
            return ServiceOrderItemType.Labor;
        }

        return ServiceOrderItemType.Part;
    }

    private static MaintenanceCategory ParseCategory(string? category)
    {
        if (string.IsNullOrWhiteSpace(category)) return MaintenanceCategory.General;

        if (Enum.TryParse<MaintenanceCategory>(category, true, out var result))
            return result;

        var lower = category.ToLowerInvariant();
        if (lower.Contains("óleo") || lower.Contains("oleo") || lower.Contains("motor") || lower.Contains("engine") || lower.Contains("radiador") || lower.Contains("arrefecimento"))
            return MaintenanceCategory.Engine;
        if (lower.Contains("freio") || lower.Contains("brake") || lower.Contains("pastilha") || lower.Contains("disco"))
            return MaintenanceCategory.Brakes;
        if (lower.Contains("pneu") || lower.Contains("tire") || lower.Contains("roda") || lower.Contains("alinhamento"))
            return MaintenanceCategory.Tires;
        if (lower.Contains("suspens") || lower.Contains("amortecedor") || lower.Contains("mola"))
            return MaintenanceCategory.Suspension;
        if (lower.Contains("câmbio") || lower.Contains("cambio") || lower.Contains("transmiss") || lower.Contains("embreagem") || lower.Contains("corrente"))
            return MaintenanceCategory.Transmission;
        if (lower.Contains("elétric") || lower.Contains("eletric") || lower.Contains("bateria") || lower.Contains("vela") || lower.Contains("lâmpada") || lower.Contains("lampada"))
            return MaintenanceCategory.Electrical;

        return MaintenanceCategory.General;
    }
}
