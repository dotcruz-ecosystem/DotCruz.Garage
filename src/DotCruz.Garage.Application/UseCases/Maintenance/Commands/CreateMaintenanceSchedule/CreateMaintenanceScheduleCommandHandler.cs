using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.ServiceOrders;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Mappers.Maintenance;
using DotCruz.Garage.Domain.Entities.ServiceOrders;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Shared.Security.Context;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.CreateMaintenanceSchedule;

public class CreateMaintenanceScheduleCommandHandler : IRequestHandler<CreateMaintenanceScheduleCommand, Guid>
{
    private readonly IVehicleWriteRepository _vehicleWriteRepository;
    private readonly IMaintenanceScheduleWriteRepository _maintenanceScheduleWriteRepository;
    private readonly IServiceOrderWriteRepository _serviceOrderWriteRepository;
    private readonly IWorkspaceProvider _workspaceProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMaintenanceScheduleCommandHandler(
        IVehicleWriteRepository vehicleWriteRepository,
        IMaintenanceScheduleWriteRepository maintenanceScheduleWriteRepository,
        IServiceOrderWriteRepository serviceOrderWriteRepository,
        IWorkspaceProvider workspaceProvider,
        IUnitOfWork unitOfWork)
    {
        _vehicleWriteRepository = vehicleWriteRepository;
        _maintenanceScheduleWriteRepository = maintenanceScheduleWriteRepository;
        _serviceOrderWriteRepository = serviceOrderWriteRepository;
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
            var serviceOrder = ServiceOrder.Create(
                workspaceId!.Value,
                userId!.Value,
                vehicle.Id,
                orderNumber: null,
                workshopName: null,
                performedAt: request.RequestDto.LastPerformedDate.Value,
                odometerAtService: request.RequestDto.LastPerformedOdometer.Value,
                totalCost: request.RequestDto.InitialCost ?? 0m,
                discount: 0,
                notes: request.RequestDto.InitialNotes);

            serviceOrder.AddItem(
                schedule.Description,
                ServiceOrderItemType.Labor,
                schedule.Category,
                quantity: 1,
                unitPrice: request.RequestDto.InitialCost ?? 0m,
                totalCost: request.RequestDto.InitialCost ?? 0m,
                maintenanceScheduleId: schedule.Id,
                notes: request.RequestDto.InitialNotes);

            await _serviceOrderWriteRepository.AddAsync(serviceOrder, cancellationToken);

            vehicle.RecordOdometerReading(request.RequestDto.LastPerformedOdometer.Value, request.RequestDto.LastPerformedDate.Value);
            schedule.RecalculateNextDue(vehicle.CurrentOdometer, vehicle.DailyMileageAverage);
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return schedule.Id;
    }
}
