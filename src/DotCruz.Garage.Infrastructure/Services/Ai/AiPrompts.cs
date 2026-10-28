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

            DIRETRIZES FUNDAMENTAIS DE EXTRAÇÃO:
            1. GERAÇÃO E FAIXA DE ANOS (yearFrom e yearTo):
               Identifique a geração exata deste modelo de acordo com o manual do proprietário oficial e informe o ano inicial (yearFrom) e o ano final (yearTo) da geração que compartilha o mesmo plano de manutenção. Se a geração ainda estiver em fabricação ativa, defina yearTo como o ano corrente ou próximo (ex: {DateTime.UtcNow.Year + 1}). NUNCA invente intervalos arbitrários.

            2. ATOMICIDADE ESTRITA DE ITENS:
               NUNCA agrupe ações ou componentes diferentes em um mesmo item (exemplo estritamente proibido: 'Troca de óleo do motor e filtro de óleo' ou 'Inspeção de freios e suspensão').
               Cada componente ou serviço DEVE ser um registro isolado com seu próprio ciclo (ex: 'Troca do óleo do motor' como um item, 'Troca do filtro de óleo do motor' como outro item independente, 'Troca do filtro de combustível' como outro item, etc.).
               Inicie sempre a descrição com verbos de ação claros: 'Troca de...', 'Inspeção de...', 'Limpeza de...', 'Ajuste de...'.

            3. INTERVALO RECORRENTE ESTÁVEL (intervalOdometer e intervalTimeMonths):
               'intervalOdometer' e 'intervalTimeMonths' DEVEM representar SEMPRE o intervalo estável recorrente de regime contínuo pós-amaciamento (ex: a cada 5.000 km / 6 meses, ou a cada 10.000 km / 12 meses).

            4. MARCOS INICIAIS GRADUAIS / AMACIAMENTO (checkpoints):
               Se o manual oficial especificar revisões preliminares com intervalos menores para amaciamento / início de vida do veículo 0KM (como em motocicletas onde a 1ª revisão é com 1.000 km / 6 meses, a 2ª com 5.000 km / 12 meses, a 3ª com 10.000 km / 18 meses, e depois estabiliza a cada 5.000 km), extraia essa progressão na lista 'checkpoints' ordenada com cada odômetro ('odometer') e o tempo em meses ('months') correspondente. Se o item não tiver marcos iniciais e sempre seguir o ciclo padrão contínuo desde 0km, retorne 'checkpoints' como lista vazia.

            Identifique também a URL de referência oficial do manual ou plano de manutenção encontrado (sourceUrl).
            """;
    }

    public static string BuildExtractServiceOrderPrompt(IEnumerable<DotCruz.Garage.Domain.Entities.Maintenance.MaintenanceSchedule> activeSchedules)
    {
        var scheduleList = activeSchedules.ToList();
        var schedulesDescription = scheduleList.Count == 0
            ? "Nenhum plano cadastrado previamente para este veículo."
            : string.Join("\n", scheduleList.Select(s => $"- Id: \"{s.Id}\" | Descrição: \"{s.Description}\" | Categoria: {s.Category}"));

        return $"""
            Você é um especialista em análise e auditoria de ordens de serviço (O.S.), orçamentos e notas fiscais automotivas no Brasil.
            Analise o documento anexo (imagem ou PDF) e extraia com máxima precisão os dados da Ordem de Serviço / Nota Fiscal para preenchimento de sistema de gestão de frotas.

            DIRETRIZES DE EXTRAÇÃO:
            1. CABEÇALHO DA OS / NOTA:
               - orderNumber: Número da ordem de serviço, orçamento ou nota fiscal (ex: "9252", "39196"). Apenas o código/número, sem prefixos como "Nº" ou "O.S.".
               - workshopName: Nome fantasia ou razão social da oficina / concessionária / prestador (ex: "MOTO SELVAGEM", "OFICINA TIO DANI").
               - performedAt: Data de emissão ou execução do serviço no formato ISO "AAAA-MM-DD" (ex: "2026-09-18"). Se o ano não estiver explícito, assuma o ano atual.
               - odometerAtService: Quilometragem (KM) do veículo registrada no momento do serviço (ex: 237971 para 237.971 km, ou 24891 para 24.891 km). Apenas o número inteiro. Se não houver KM anotado, retorne nulo.
               - totalCost: Valor total final da OS / nota fiscal (ex: 864.00 ou 178.70).
               - discount: Valor total de desconto concedido em R$, se houver (caso contrário, 0).
               - notes: Observações gerais, solicitações do cliente ou defeitos relatados no documento (ex: "Defeito: pedal de freio está caído").
               - detectedVehiclePlate: Placa do veículo identificada no documento (ex: "MNG-8655", "TAL-1G31"). Se não houver, retorne nulo.
               - detectedVehicleModel: Modelo ou descrição do veículo identificada no documento (ex: "CORSA 95 1.4 PRETO", "YAMAHA MT 03").

            2. ITENS (PEÇAS E MÃO DE OBRA / SERVIÇOS):
               Para cada linha de peça ou serviço discriminada no documento:
               - description: Mantenha a descrição fiel ao texto do documento (ex: "PARAFUSO PEDAL CAMBIO FAZER 250", "TROCA VELA IGNICAO 250/500cc").
               - type: "Part" se for peça/produto físico/óleo/fluido/abraçadeira/parafuso, ou "Labor" se for serviço/mão de obra/instalação/troca/revisão.
               - category: Categoria do componente/serviço. Deve ser estritamente uma destas opções:
                 * "Engine" (óleo, filtro de óleo, filtro de ar, vela de ignição, correia, arrefecimento, motor, radiador, mangueiras, cebolão)
                 * "Brakes" (pastilha, disco, fluido de freio, lona, pedal de freio)
                 * "Tires" (pneu, alinhamento, balanceamento, câmara, roda)
                 * "Suspension" (amortecedor, mola, bucha, pivô, terminal, suporte)
                 * "Transmission" (embreagem, cabo, relação, corrente, câmbio)
                 * "Electrical" (bateria, alternador, lâmpada, fiação, chicote)
                 * "General" (outros itens, pedaleira, parafusos gerais, higienização, estética)
               - quantity: Quantidade discriminada (ex: 1, 7). Se não especificada, assuma 1.
               - unitPrice: Preço unitário do item em R$. Se não estiver discriminado, use o totalCost dividido pela quantidade.
               - totalCost: Valor total daquela linha em R$.
               - partNumber: Código ou referência de catálogo da peça se houver no documento (ex: "136592MGL", "136164"). Caso contrário, nulo.
               - matchedScheduleId: Se este item corresponder a um dos planos preventivos existentes listados abaixo, informe o GUID exato do plano. Caso contrário, retorne nulo:
                 {schedulesDescription}
            """;
    }
}
