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
    private readonly IWorkspaceProvider _workspaceProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteMaintenanceScheduleCommandHandler(
        IVehicleWriteRepository vehicleWriteRepository,
        IMaintenanceScheduleWriteRepository maintenanceScheduleWriteRepository,
        IMaintenanceLogWriteRepository maintenanceLogWriteRepository,
        IWorkspaceProvider workspaceProvider,
        IUnitOfWork unitOfWork)
    {
        _vehicleWriteRepository = vehicleWriteRepository;
        _maintenanceScheduleWriteRepository = maintenanceScheduleWriteRepository;
        _maintenanceLogWriteRepository = maintenanceLogWriteRepository;
        _workspaceProvider = workspaceProvider;
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

        var workspaceId = _workspaceProvider.WorkspaceId ?? schedule.WorkspaceId;
        var userId = _workspaceProvider.UserId ?? schedule.UserId;

        schedule.Perform(request.RequestDto.OdometerAtService, request.RequestDto.PerformedAt, vehicle.DailyMileageAverage);

        vehicle.RecordOdometerReading(request.RequestDto.OdometerAtService, request.RequestDto.PerformedAt);

        var schedules = await _maintenanceScheduleWriteRepository.GetByVehicleIdToUpdateAsync(vehicle.Id, cancellationToken);
        if (schedules is not null)
        {
            foreach (var otherSchedule in schedules)
            {
                if (otherSchedule.Id != schedule.Id)
                    otherSchedule.RecalculateNextDue(vehicle.CurrentOdometer, vehicle.DailyMileageAverage);
            }
        }

        var log = MaintenanceLog.Create(
            workspaceId,
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
