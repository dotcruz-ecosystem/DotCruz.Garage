using DotCruz.Garage.Application.UseCases.Blueprints.Commands.ProcessPendingBlueprints;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotCruz.Garage.Worker.BackgroundServices;

public class VehicleBlueprintPollerWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<VehicleBlueprintPollerWorker> _logger;
    private readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(15);

    public VehicleBlueprintPollerWorker(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<VehicleBlueprintPollerWorker> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("VehicleBlueprintPollerWorker iniciado, verificando blueprints pendentes a cada {Seconds}s.", _pollInterval.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await mediator.Send(new ProcessPendingBlueprintsCommand(Limit: 5), stoppingToken);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Erro ao processar lote de blueprints pendentes.");
            }

            try
            {
                await Task.Delay(_pollInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation("VehicleBlueprintPollerWorker finalizado.");
    }
}
