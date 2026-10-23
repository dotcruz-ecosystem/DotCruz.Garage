using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.ServiceOrders;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.DTOs.Maintenance;
using DotCruz.Garage.Application.DTOs.ServiceOrders;
using DotCruz.Garage.Application.Mappers.ServiceOrders;
using DotCruz.Garage.Domain.Entities.ServiceOrders;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Shared.Security.Context;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.CompleteMaintenanceSchedule;

public class CompleteMaintenanceScheduleCommandHandler : IRequestHandler<CompleteMaintenanceScheduleCommand, ServiceOrderDto>
{
    private readonly IVehicleWriteRepository _vehicleWriteRepository;
    private readonly IMaintenanceScheduleWriteRepository _maintenanceScheduleWriteRepository;
    private readonly IServiceOrderWriteRepository _serviceOrderWriteRepository;
    private readonly IWorkspaceProvider _workspaceProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteMaintenanceScheduleCommandHandler(
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

    public async Task<ServiceOrderDto> Handle(CompleteMaintenanceScheduleCommand request, CancellationToken cancellationToken)
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

        var serviceOrder = ServiceOrder.Create(
            workspaceId,
            userId,
            vehicle.Id,
            orderNumber: null,
            workshopName: null,
            performedAt: request.RequestDto.PerformedAt,
            odometerAtService: request.RequestDto.OdometerAtService,
            totalCost: request.RequestDto.Cost,
            discount: 0,
            notes: request.RequestDto.Notes);

        serviceOrder.AddItem(
            schedule.Description,
            ServiceOrderItemType.Labor,
            schedule.Category,
            quantity: 1,
            unitPrice: request.RequestDto.Cost,
            totalCost: request.RequestDto.Cost,
            maintenanceScheduleId: schedule.Id,
            notes: request.RequestDto.Notes);

        if (request.RequestDto.Attachments != null)
        {
            foreach (var att in request.RequestDto.Attachments)
            {
                serviceOrder.AddAttachment(att.FileName, att.FileUrl, att.ContentType, att.FileSize);
            }
        }

        await _serviceOrderWriteRepository.AddAsync(serviceOrder, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return serviceOrder.ToDto();
    }
}
