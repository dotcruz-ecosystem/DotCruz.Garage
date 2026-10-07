using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Shared.Security.Context;
using DotCruz.Garage.Application.Mappers.Maintenance;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Garage.Domain.Services;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.LogMaintenance;

public class LogMaintenanceCommandHandler : IRequestHandler<LogMaintenanceCommand, Guid>
{
    private readonly IVehicleWriteRepository _vehicleWriteRepository;
    private readonly IMaintenanceScheduleWriteRepository _maintenanceScheduleWriteRepository;
    private readonly IMaintenanceLogWriteRepository _maintenanceLogWriteRepository;
    private readonly IWorkspaceProvider _workspaceProvider;
    private readonly IUnitOfWork _unitOfWork;

    public LogMaintenanceCommandHandler(
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

    public async Task<Guid> Handle(LogMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleWriteRepository.GetByIdWithHistoriesToUpdateAsync(request.VehicleId, cancellationToken) 
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        if (request.RequestDto.OdometerAtService < vehicle.CurrentOdometer)
            throw new ErrorOnValidationException(ResourceMessagesException.ODOMETER_VALUE_INVALID);

        var workspaceId = _workspaceProvider.WorkspaceId;
        var userId = _workspaceProvider.UserId;

        if (request.RequestDto.MaintenanceScheduleId.HasValue)
        {
            var schedule = await _maintenanceScheduleWriteRepository.GetByIdToUpdateAsync(request.RequestDto.MaintenanceScheduleId.Value, cancellationToken);
            schedule?.Perform(request.RequestDto.OdometerAtService, request.RequestDto.PerformedAt, vehicle.DailyMileageAverage);
        }

        vehicle.UpdateOdometer(request.RequestDto.OdometerAtService, request.RequestDto.PerformedAt);
        vehicle.OdometerHistories.Add(OdometerHistory.Create(vehicle.Id, request.RequestDto.OdometerAtService, request.RequestDto.PerformedAt));

        var newAverage = OdometerCalculator.CalculateDailyAverage(
            vehicle.OdometerHistories,
            vehicle.DailyMileageAverage
        );

        vehicle.UpdateDailyMileageAverage(newAverage);

        var schedules = await _maintenanceScheduleWriteRepository.GetByVehicleIdToUpdateAsync(vehicle.Id, cancellationToken);
        foreach (var schedule in schedules)
        {
            if (schedule.Id != request.RequestDto.MaintenanceScheduleId)
                schedule.RecalculateNextDue(vehicle.CurrentOdometer, vehicle.DailyMileageAverage);
        }

        var logMaintenance = request.ToDomain(workspaceId!.Value, userId!.Value);

        await _maintenanceLogWriteRepository.AddAsync(logMaintenance, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return logMaintenance.Id;
    }
}
