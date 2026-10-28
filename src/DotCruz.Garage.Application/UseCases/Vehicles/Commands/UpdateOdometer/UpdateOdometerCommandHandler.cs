using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Garage.Domain.Services;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.UpdateOdometer;

public class UpdateOdometerCommandHandler : IRequestHandler<UpdateOdometerCommand>
{
    private readonly IVehicleWriteRepository _vehicleWriteRepository;
    private readonly IMaintenanceScheduleWriteRepository _maintenanceScheduleWriteRepository;
    private readonly DotCruz.Shared.Security.Context.IWorkspaceProvider _workspaceProvider;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOdometerCommandHandler(
        IVehicleWriteRepository vehicleWriteRepository,
        IMaintenanceScheduleWriteRepository maintenanceScheduleWriteRepository,
        DotCruz.Shared.Security.Context.IWorkspaceProvider workspaceProvider,
        IUnitOfWork unitOfWork)
    {
        _vehicleWriteRepository = vehicleWriteRepository;
        _maintenanceScheduleWriteRepository = maintenanceScheduleWriteRepository;
        _workspaceProvider = workspaceProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateOdometerCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleWriteRepository.GetByIdWithHistoriesToUpdateAsync(request.VehicleId, cancellationToken) 
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var workspaceId = _workspaceProvider.WorkspaceId;
        if (workspaceId.HasValue && !vehicle.CanWrite(workspaceId.Value))
        {
            throw new ForbiddenException(ResourceMessagesException.VEHICLE_ACCESS_DENIED);
        }
        
        var recordedAt = request.RequestDto.RecordedAt ?? DateTimeOffset.UtcNow;

        if (request.RequestDto.NewOdometer < vehicle.CurrentOdometer)
            throw new ErrorOnValidationException(ResourceMessagesException.ODOMETER_VALUE_INVALID);

        vehicle.RecordOdometerReading(request.RequestDto.NewOdometer, recordedAt);

        var schedules = await _maintenanceScheduleWriteRepository.GetByVehicleIdToUpdateAsync(vehicle.Id, cancellationToken);
        if (schedules != null)
        {
            foreach (var schedule in schedules)
                schedule.RecalculateNextDue(vehicle.CurrentOdometer, vehicle.DailyMileageAverage);
        }

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
