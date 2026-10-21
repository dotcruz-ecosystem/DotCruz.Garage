using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Mappers.Maintenance;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Garage.Domain.Services;
using DotCruz.Shared.Security.Context;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.CreateMaintenanceSchedule;

public class CreateMaintenanceScheduleCommandHandler : IRequestHandler<CreateMaintenanceScheduleCommand, Guid>
{
    private readonly IVehicleWriteRepository _vehicleWriteRepository;
    private readonly IMaintenanceScheduleWriteRepository _maintenanceScheduleWriteRepository;
    private readonly IMaintenanceLogWriteRepository _maintenanceLogWriteRepository;
    private readonly IWorkspaceProvider _workspaceProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMaintenanceScheduleCommandHandler(
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

    public async Task<Guid> Handle(CreateMaintenanceScheduleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleWriteRepository.GetByIdWithHistoriesToUpdateAsync(request.VehicleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);
        
        var workspaceId = _workspaceProvider.WorkspaceId;
        var userId = _workspaceProvider.UserId;

        var schedule = request.ToDomain(workspaceId!.Value, userId!.Value, vehicle.CurrentOdometer, vehicle.DailyMileageAverage);

        await _maintenanceScheduleWriteRepository.AddAsync(schedule, cancellationToken);

        if (request.RequestDto.RecordInitialLog && 
            request.RequestDto.LastPerformedDate.HasValue && 
            request.RequestDto.LastPerformedOdometer.HasValue)
        {
            var initialLog = MaintenanceLog.Create(
                workspaceId!.Value,
                userId!.Value,
                vehicle.Id,
                schedule.Id,
                schedule.Description,
                request.RequestDto.LastPerformedDate.Value,
                request.RequestDto.LastPerformedOdometer.Value,
                request.RequestDto.InitialCost ?? 0m,
                request.RequestDto.InitialReceiptUrl,
                request.RequestDto.InitialNotes
            );

            await _maintenanceLogWriteRepository.AddAsync(initialLog, cancellationToken);

            vehicle.RecordOdometerReading(request.RequestDto.LastPerformedOdometer.Value, request.RequestDto.LastPerformedDate.Value);
            schedule.RecalculateNextDue(vehicle.CurrentOdometer, vehicle.DailyMileageAverage);
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return schedule.Id;
    }
}
