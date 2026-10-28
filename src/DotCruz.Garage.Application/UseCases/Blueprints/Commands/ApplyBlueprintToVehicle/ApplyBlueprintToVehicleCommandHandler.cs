using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Shared.Security.Context;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Blueprints.Commands.ApplyBlueprintToVehicle;

public class ApplyBlueprintToVehicleCommandHandler : IRequestHandler<ApplyBlueprintToVehicleCommand, IReadOnlyCollection<Guid>>
{
    private readonly IVehicleWriteRepository _vehicleWriteRepository;
    private readonly IVehicleBlueprintRepository _blueprintRepository;
    private readonly IMaintenanceScheduleReadRepository _scheduleReadRepository;
    private readonly IMaintenanceScheduleWriteRepository _scheduleWriteRepository;
    private readonly IWorkspaceProvider _workspaceProvider;
    private readonly IUnitOfWork _unitOfWork;

    public ApplyBlueprintToVehicleCommandHandler(
        IVehicleWriteRepository vehicleWriteRepository,
        IVehicleBlueprintRepository blueprintRepository,
        IMaintenanceScheduleReadRepository scheduleReadRepository,
        IMaintenanceScheduleWriteRepository scheduleWriteRepository,
        IWorkspaceProvider workspaceProvider,
        IUnitOfWork unitOfWork)
    {
        _vehicleWriteRepository = vehicleWriteRepository;
        _blueprintRepository = blueprintRepository;
        _scheduleReadRepository = scheduleReadRepository;
        _scheduleWriteRepository = scheduleWriteRepository;
        _workspaceProvider = workspaceProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyCollection<Guid>> Handle(ApplyBlueprintToVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleWriteRepository.GetByIdWithHistoriesToUpdateAsync(request.VehicleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var blueprint = await _blueprintRepository.FindMatchingBlueprintAsync(
            vehicle.Brand,
            vehicle.Model,
            vehicle.Engine,
            vehicle.Year,
            vehicle.Type,
            cancellationToken);

        if (blueprint is null || blueprint.Status != BlueprintStatus.Ready)
        {
            throw new ErrorOnValidationException("O plano de manutenção para este modelo ainda não foi processado ou finalizado.");
        }

        var existingSchedules = await _scheduleReadRepository.GetByVehicleIdAsync(vehicle.Id, cancellationToken);
        var existingDescriptions = existingSchedules
            .Select(s => s.Description.Trim().ToLower())
            .ToHashSet();

        var workspaceId = _workspaceProvider.WorkspaceId!.Value;
        var userId = _workspaceProvider.UserId!.Value;
        var createdScheduleIds = new List<Guid>();

        foreach (var item in request.RequestDto.Items)
        {
            var template = blueprint.Templates.FirstOrDefault(t => t.Id == item.TemplateId);
            if (template is null) continue;

            if (existingDescriptions.Contains(template.Description.Trim().ToLower()))
            {
                // Já existe agendamento ativo ou pendente com esta descrição para o veículo
                continue;
            }

            int? lastOdo;
            DateTimeOffset? lastDate;

            if (item.CalculationMode == "NeverPerformed")
            {
                // Para itens nunca realizados / desde zero: odômetro 0 e data de criação/início do veículo
                lastOdo = 0;
                lastDate = vehicle.CreatedAt;
            }
            else if (item.CalculationMode == "RecentlyPerformed")
            {
                lastOdo = item.LastPerformedOdometer ?? vehicle.CurrentOdometer;
                lastDate = item.LastPerformedDate ?? DateTimeOffset.UtcNow;
            }
            else
            {
                // PendingFirstExecution
                lastOdo = null;
                lastDate = null;
            }

            var schedule = MaintenanceSchedule.Create(
                workspaceId,
                userId,
                vehicle.Id,
                template.Description,
                template.Category,
                template.IntervalOdometer,
                template.IntervalTimeMonths,
                lastOdo,
                lastDate,
                warningThresholdOdometer: null,
                warningThresholdDays: null,
                vehicle.CurrentOdometer,
                vehicle.DailyMileageAverage
            );

            if (template.Checkpoints.Any())
            {
                foreach (var cp in template.Checkpoints.OrderBy(c => c.Order))
                {
                    bool isAlreadyCompleted = (lastOdo.HasValue && lastOdo.Value >= cp.Odometer) ||
                                              (item.CalculationMode == "NeverPerformed" && vehicle.CurrentOdometer >= cp.Odometer);

                    DateTimeOffset? completedAt = isAlreadyCompleted ? (lastDate ?? vehicle.CreatedAt) : null;
                    int? completedOdo = isAlreadyCompleted ? Math.Min(vehicle.CurrentOdometer, cp.Odometer) : null;

                    schedule.AddCheckpoint(
                        cp.Odometer,
                        cp.Months,
                        cp.Order,
                        isCompleted: isAlreadyCompleted,
                        completedAt: completedAt,
                        completedOdometer: completedOdo);
                }

                if (lastOdo.HasValue || lastDate.HasValue)
                {
                    schedule.RecalculateNextDue(vehicle.CurrentOdometer, vehicle.DailyMileageAverage);
                }
            }

            await _scheduleWriteRepository.AddAsync(schedule, cancellationToken);
            createdScheduleIds.Add(schedule.Id);
            existingDescriptions.Add(template.Description.Trim().ToLower());
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return createdScheduleIds;
    }
}
