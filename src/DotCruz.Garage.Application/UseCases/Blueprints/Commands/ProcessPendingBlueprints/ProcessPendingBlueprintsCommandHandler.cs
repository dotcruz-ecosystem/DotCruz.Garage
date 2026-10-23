using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.UseCases.Blueprints.Commands.ProcessVehicleBlueprint;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DotCruz.Garage.Application.UseCases.Blueprints.Commands.ProcessPendingBlueprints;

public class ProcessPendingBlueprintsCommandHandler : IRequestHandler<ProcessPendingBlueprintsCommand, int>
{
    private readonly IVehicleBlueprintRepository _blueprintRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessPendingBlueprintsCommandHandler> _logger;

    public ProcessPendingBlueprintsCommandHandler(
        IVehicleBlueprintRepository blueprintRepository,
        IMediator mediator,
        ILogger<ProcessPendingBlueprintsCommandHandler> logger)
    {
        _blueprintRepository = blueprintRepository;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<int> Handle(ProcessPendingBlueprintsCommand request, CancellationToken cancellationToken)
    {
        var pendingList = await _blueprintRepository.GetPendingBlueprintsAsync(request.Limit, cancellationToken);
        if (pendingList.Count == 0)
        {
            return 0;
        }

        _logger.LogInformation("Encontrados {Count} blueprints pendentes para extração de manual IA.", pendingList.Count);

        var processedCount = 0;
        foreach (var blueprint in pendingList)
        {
            try
            {
                await _mediator.Send(new ProcessVehicleBlueprintCommand(blueprint.Id), cancellationToken);
                processedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao processar blueprint {BlueprintId}.", blueprint.Id);
            }
        }

        return processedCount;
    }
}
