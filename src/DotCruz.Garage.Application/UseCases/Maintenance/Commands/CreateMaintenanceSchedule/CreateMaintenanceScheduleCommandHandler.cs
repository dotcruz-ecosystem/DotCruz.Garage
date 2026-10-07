using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Shared.Security.Context;
using DotCruz.Garage.Application.Mappers.Maintenance;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.CreateMaintenanceSchedule;

public class CreateMaintenanceScheduleCommandHandler : IRequestHandler<CreateMaintenanceScheduleCommand, Guid>
{
    private readonly IVehicleReadRepository _vehicleReadRepository;
    private readonly IMaintenanceScheduleWriteRepository _maintenanceScheduleWriteRepository;
    private readonly IWorkspaceProvider _workspaceProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMaintenanceScheduleCommandHandler(
        IVehicleReadRepository vehicleReadRepository,
        IMaintenanceScheduleWriteRepository maintenanceScheduleWriteRepository,
        IWorkspaceProvider workspaceProvider,
        IUnitOfWork unitOfWork)
    {
        _vehicleReadRepository = vehicleReadRepository;
        _maintenanceScheduleWriteRepository = maintenanceScheduleWriteRepository;
        _workspaceProvider = workspaceProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateMaintenanceScheduleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleReadRepository.GetByIdAsync(request.VehicleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);
        
        var workspaceId = _workspaceProvider.WorkspaceId;
        var userId = _workspaceProvider.UserId;

        var schedule = request.ToDomain(workspaceId!.Value, userId!.Value, vehicle.CurrentOdometer, vehicle.DailyMileageAverage);

        await _maintenanceScheduleWriteRepository.AddAsync(schedule, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return schedule.Id;
    }
}
