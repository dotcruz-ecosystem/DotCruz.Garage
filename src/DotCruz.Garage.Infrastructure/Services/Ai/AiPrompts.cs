namespace DotCruz.Garage.Infrastructure.Services.Ai;

public static class AiPrompts
{
    public static string BuildExtractMaintenancePlanPrompt(string vehicleTypeStr, string brand, string model, string engine, int year)
    {
        return $"""
            Você é um engenheiro automotivo especialista em manutenção preventiva e frotas de veículos no Brasil.
            Pesquise na internet utilizando a busca oficial da montadora e analise o manual do proprietário oficial e o plano de revisões periódicas para:
            - Tipo: {vehicleTypeStr}
            - Marca: {brand}
            - Modelo: {model}
            - Motorização: {engine}
            - Ano de Referência: {year}

            Identifique a geração deste modelo (faixa de anos compatíveis, ex: {year - 2} a {year + 3}) e extraia detalhadamente todas as manutenções preventivas periódicas recomendadas pelo fabricante (troca de óleo do motor, filtros, fluido de freio, velas, correia dentada, inspeções de suspensão e freios, etc.).
            Para cada item, identifique o intervalo em quilômetros (intervalOdometer) e em meses (intervalTimeMonths), sua categoria e se é obrigatório.
            Identifique também a URL de referência oficial do manual ou plano de manutenção encontrado.
            """;
    }
}
