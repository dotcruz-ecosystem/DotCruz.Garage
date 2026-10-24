using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Abstractions.Services.Ai;
using DotCruz.Garage.Application.Abstractions.Services.Auth;
using DotCruz.Garage.Application.Abstractions.Services.Notifications;
using DotCruz.Garage.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DotCruz.Garage.Application.UseCases.Blueprints.Commands.ProcessVehicleBlueprint;

public class ProcessVehicleBlueprintCommandHandler : IRequestHandler<ProcessVehicleBlueprintCommand>
{
    private readonly IVehicleBlueprintRepository _blueprintRepository;
    private readonly IVehicleReadRepository _vehicleReadRepository;
    private readonly IAuthServiceClient _authServiceClient;
    private readonly INotificationService _notificationService;
    private readonly IAiManualExtractionService _aiService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessVehicleBlueprintCommandHandler> _logger;

    public ProcessVehicleBlueprintCommandHandler(
        IVehicleBlueprintRepository blueprintRepository,
        IVehicleReadRepository vehicleReadRepository,
        IAuthServiceClient authServiceClient,
        INotificationService notificationService,
        IAiManualExtractionService aiService,
        IUnitOfWork unitOfWork,
        ILogger<ProcessVehicleBlueprintCommandHandler> logger)
    {
        _blueprintRepository = blueprintRepository;
        _vehicleReadRepository = vehicleReadRepository;
        _authServiceClient = authServiceClient;
        _notificationService = notificationService;
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
            var targetYear = blueprint.YearTo > 0 && blueprint.YearFrom > 0
                ? (blueprint.YearFrom + blueprint.YearTo) / 2
                : (blueprint.YearTo > 0 ? blueprint.YearTo : blueprint.YearFrom);

            var extracted = await _aiService.ExtractMaintenancePlanAsync(
                blueprint.Brand,
                blueprint.Model,
                blueprint.Engine,
                targetYear > 0 ? targetYear : DateTime.UtcNow.Year,
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

            var finalYearFrom = blueprint.YearFrom > 0 && extracted.YearFrom > 0
                ? Math.Min(blueprint.YearFrom, extracted.YearFrom)
                : (extracted.YearFrom > 0 ? extracted.YearFrom : blueprint.YearFrom);

            var finalYearTo = Math.Max(blueprint.YearTo, extracted.YearTo > 0 ? extracted.YearTo : blueprint.YearTo);

            blueprint.MarkAsReady(extracted.SourceUrl, finalYearFrom, finalYearTo);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Blueprint {BlueprintId} processado com sucesso com {Count} templates. Faixa de anos: {From} a {To}.", blueprint.Id, extracted.Items.Count, finalYearFrom, finalYearTo);

            try
            {
                var matchingVehicles = await _vehicleReadRepository.GetVehiclesByAttributesAsync(
                    blueprint.Brand,
                    blueprint.Model,
                    blueprint.Engine,
                    blueprint.YearFrom,
                    blueprint.YearTo,
                    cancellationToken);

                _logger.LogInformation("Encontrados {Count} veículos compatíveis para notificar sobre o blueprint {BlueprintId}.", matchingVehicles.Count, blueprint.Id);

                foreach (var vehicle in matchingVehicles)
                {
                    try
                    {
                        var user = await _authServiceClient.GetUserByIdAsync(vehicle.UserId, vehicle.WorkspaceId, cancellationToken);
                        if (user != null && !string.IsNullOrWhiteSpace(user.Email))
                        {
                            await _notificationService.SendBlueprintReadyNotificationAsync(
                                vehicle.UserId,
                                vehicle.WorkspaceId,
                                user.Name,
                                user.Email,
                                vehicle.Id,
                                vehicle.Name,
                                extracted.Items.Count,
                                cancellationToken);

                            _logger.LogInformation("Notificação enviada com sucesso para usuário {UserId} ({Email}) do veículo {VehicleId}.", vehicle.UserId, user.Email, vehicle.Id);
                        }
                        else
                        {
                            _logger.LogWarning("Usuário {UserId} não encontrado ou sem e-mail para veículo {VehicleId}.", vehicle.UserId, vehicle.Id);
                        }
                    }
                    catch (Exception exUser)
                    {
                        _logger.LogWarning(exUser, "Erro ao enviar notificação de blueprint pronto para veículo {VehicleId}", vehicle.Id);
                    }
                }
            }
            catch (Exception exVehicles)
            {
                _logger.LogWarning(exVehicles, "Erro ao buscar veículos compatíveis para notificar sobre o blueprint {BlueprintId}", blueprint.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar blueprint {BlueprintId}.", request.BlueprintId);
            blueprint.MarkAsFailed(ex.Message);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
