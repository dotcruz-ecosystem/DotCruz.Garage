using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Catalog;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Catalog.Commands.ApplyBlueprintToVehicle;

public class ApplyBlueprintToVehicleCommandHandler : IRequestHandler<ApplyBlueprintToVehicleCommand, int>
{
    private readonly IVehicleReadRepository _vehicleReadRepository;
    private readonly IVehicleBlueprintReadRepository _blueprintReadRepository;
    private readonly IMaintenanceScheduleWriteRepository _maintenanceScheduleWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApplyBlueprintToVehicleCommandHandler(
        IVehicleReadRepository vehicleReadRepository,
        IVehicleBlueprintReadRepository blueprintReadRepository,
        IMaintenanceScheduleWriteRepository maintenanceScheduleWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleReadRepository = vehicleReadRepository;
        _blueprintReadRepository = blueprintReadRepository;
        _maintenanceScheduleWriteRepository = maintenanceScheduleWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(ApplyBlueprintToVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleReadRepository.GetByIdAsync(request.VehicleId, cancellationToken);
        if (vehicle is null)
        {
            throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);
        }

        var blueprint = await _blueprintReadRepository.GetByIdWithTemplatesAsync(request.BlueprintId, cancellationToken);
        if (blueprint is null)
        {
            throw new NotFoundException(ResourceMessagesException.BLUEPRINT_NOT_FOUND);
        }

        int count = 0;
        foreach (var template in blueprint.PlanTemplates)
        {
            var schedule = MaintenanceSchedule.Create(
                tenantId: vehicle.TenantId,
                userId: vehicle.UserId,
                vehicleId: vehicle.Id,
                description: template.Description,
                category: template.Category,
                intervalOdometer: template.IntervalOdometer,
                intervalTimeMonths: template.IntervalTimeMonths,
                lastPerformedOdometer: null,
                lastPerformedDate: null,
                warningThresholdOdometer: null,
                warningThresholdDays: null,
                currentOdometer: vehicle.CurrentOdometer,
                dailyMileageAverage: vehicle.DailyMileageAverage);

            await _maintenanceScheduleWriteRepository.AddAsync(schedule, cancellationToken);
            count++;
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return count;
    }
}
