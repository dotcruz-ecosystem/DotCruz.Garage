using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Abstractions.Services.Ai;
using DotCruz.Garage.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DotCruz.Garage.Application.UseCases.Blueprints.Commands.ProcessVehicleBlueprint;

public class ProcessVehicleBlueprintCommandHandler : IRequestHandler<ProcessVehicleBlueprintCommand>
{
    private readonly IVehicleBlueprintRepository _blueprintRepository;
    private readonly IAiManualExtractionService _aiService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessVehicleBlueprintCommandHandler> _logger;

    public ProcessVehicleBlueprintCommandHandler(
        IVehicleBlueprintRepository blueprintRepository,
        IAiManualExtractionService aiService,
        IUnitOfWork unitOfWork,
        ILogger<ProcessVehicleBlueprintCommandHandler> logger)
    {
        _blueprintRepository = blueprintRepository;
        _aiService = aiService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ProcessVehicleBlueprintCommand request, CancellationToken cancellationToken)
    {
        var blueprint = await _blueprintRepository.GetByIdWithTemplatesAsync(request.BlueprintId, cancellationToken);
        if (blueprint is null)
        {
            _logger.LogWarning("Blueprint com Id {BlueprintId} não encontrado para processamento.", request.BlueprintId);
            return;
        }

        if (blueprint.Status == BlueprintStatus.Ready && blueprint.Templates.Any())
        {
            _logger.LogInformation("Blueprint {BlueprintId} já está processado e pronto.", request.BlueprintId);
            return;
        }

        blueprint.MarkAsProcessing();
        await _unitOfWork.CommitAsync(cancellationToken);

        try
        {
            var extracted = await _aiService.ExtractMaintenancePlanAsync(
                blueprint.Brand,
                blueprint.Model,
                blueprint.Engine,
                blueprint.YearFrom,
                blueprint.Type,
                cancellationToken);

            foreach (var item in extracted.Items)
            {
                blueprint.AddTemplate(
                    item.Description,
                    item.Category,
                    item.IntervalOdometer,
                    item.IntervalTimeMonths,
                    item.IsRequired);
            }

            blueprint.MarkAsReady(extracted.SourceUrl, extracted.YearFrom, extracted.YearTo);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Blueprint {BlueprintId} processado com sucesso com {Count} templates.", blueprint.Id, extracted.Items.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar blueprint {BlueprintId}.", request.BlueprintId);
            blueprint.MarkAsFailed(ex.Message);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
