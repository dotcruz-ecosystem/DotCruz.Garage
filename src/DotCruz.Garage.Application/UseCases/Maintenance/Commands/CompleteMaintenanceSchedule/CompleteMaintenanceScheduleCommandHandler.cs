using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.DTOs.Maintenance;
using DotCruz.Garage.Application.Mappers.Maintenance;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Garage.Domain.Services;
using DotCruz.Shared.Security.Context;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.CompleteMaintenanceSchedule;

public class CompleteMaintenanceScheduleCommandHandler : IRequestHandler<CompleteMaintenanceScheduleCommand, MaintenanceLogDto>
{
    private readonly IVehicleWriteRepository _vehicleWriteRepository;
    private readonly IMaintenanceScheduleWriteRepository _maintenanceScheduleWriteRepository;
    private readonly IMaintenanceLogWriteRepository _maintenanceLogWriteRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteMaintenanceScheduleCommandHandler(
        IVehicleWriteRepository vehicleWriteRepository,
        IMaintenanceScheduleWriteRepository maintenanceScheduleWriteRepository,
        IMaintenanceLogWriteRepository maintenanceLogWriteRepository,
        ITenantProvider tenantProvider,
        IUnitOfWork unitOfWork)
    {
        _vehicleWriteRepository = vehicleWriteRepository;
        _maintenanceScheduleWriteRepository = maintenanceScheduleWriteRepository;
        _maintenanceLogWriteRepository = maintenanceLogWriteRepository;
        _tenantProvider = tenantProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<MaintenanceLogDto> Handle(CompleteMaintenanceScheduleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleWriteRepository.GetByIdWithHistoriesToUpdateAsync(request.VehicleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var schedule = await _maintenanceScheduleWriteRepository.GetByIdToUpdateAsync(request.ScheduleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.MAINTENANCE_SCHEDULE_NOT_FOUND);

        if (schedule.VehicleId != vehicle.Id)
            throw new NotFoundException(ResourceMessagesException.MAINTENANCE_SCHEDULE_NOT_FOUND);

        if (request.RequestDto.OdometerAtService < vehicle.CurrentOdometer)
            throw new ErrorOnValidationException(ResourceMessagesException.ODOMETER_VALUE_INVALID);

        var tenantId = _tenantProvider.TenantId ?? schedule.TenantId;
        var userId = _tenantProvider.UserId ?? schedule.UserId;

        schedule.Perform(request.RequestDto.OdometerAtService, request.RequestDto.PerformedAt, vehicle.DailyMileageAverage);

        if (request.RequestDto.OdometerAtService > vehicle.CurrentOdometer)
        {
            vehicle.UpdateOdometer(request.RequestDto.OdometerAtService, request.RequestDto.PerformedAt);
            vehicle.OdometerHistories.Add(OdometerHistory.Create(vehicle.Id, request.RequestDto.OdometerAtService, request.RequestDto.PerformedAt));

            var newAverage = OdometerCalculator.CalculateDailyAverage(
                vehicle.OdometerHistories,
                vehicle.DailyMileageAverage);
            vehicle.UpdateDailyMileageAverage(newAverage);

            var schedules = await _maintenanceScheduleWriteRepository.GetByVehicleIdToUpdateAsync(vehicle.Id, cancellationToken);
            foreach (var otherSchedule in schedules)
            {
                if (otherSchedule.Id != schedule.Id)
                    otherSchedule.RecalculateNextDue(vehicle.CurrentOdometer, vehicle.DailyMileageAverage);
            }
        }

        var log = MaintenanceLog.Create(
            tenantId,
            userId,
            vehicle.Id,
            schedule.Id,
            schedule.Description,
            request.RequestDto.PerformedAt,
            request.RequestDto.OdometerAtService,
            request.RequestDto.Cost,
            request.RequestDto.ReceiptUrl,
            request.RequestDto.Notes);

        await _maintenanceLogWriteRepository.AddAsync(log, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return log.ToDto();
    }
}
