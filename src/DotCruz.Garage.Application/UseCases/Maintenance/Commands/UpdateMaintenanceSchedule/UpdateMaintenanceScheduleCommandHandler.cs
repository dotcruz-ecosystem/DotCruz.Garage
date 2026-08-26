using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.UpdateMaintenanceSchedule;

public class UpdateMaintenanceScheduleCommandHandler : IRequestHandler<UpdateMaintenanceScheduleCommand>
{
    private readonly IVehicleReadRepository _vehicleReadRepository;
    private readonly IMaintenanceScheduleWriteRepository _maintenanceScheduleWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMaintenanceScheduleCommandHandler(
        IVehicleReadRepository vehicleReadRepository,
        IMaintenanceScheduleWriteRepository maintenanceScheduleWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleReadRepository = vehicleReadRepository;
        _maintenanceScheduleWriteRepository = maintenanceScheduleWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateMaintenanceScheduleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleReadRepository.GetByIdAsync(request.VehicleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var schedule = await _maintenanceScheduleWriteRepository.GetByIdToUpdateAsync(request.ScheduleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.MAINTENANCE_SCHEDULE_NOT_FOUND);

        if (schedule.VehicleId != vehicle.Id)
            throw new NotFoundException(ResourceMessagesException.MAINTENANCE_SCHEDULE_NOT_FOUND);

        schedule.Update(
            request.RequestDto.Description,
            request.RequestDto.Category,
            request.RequestDto.IntervalOdometer,
            request.RequestDto.IntervalTimeMonths,
            request.RequestDto.WarningThresholdOdometer,
            request.RequestDto.WarningThresholdDays,
            vehicle.CurrentOdometer,
            vehicle.DailyMileageAverage);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
