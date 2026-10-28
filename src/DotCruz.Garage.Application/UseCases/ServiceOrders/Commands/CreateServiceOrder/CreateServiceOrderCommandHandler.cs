using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.ServiceOrders;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.DTOs.ServiceOrders;
using DotCruz.Garage.Application.Mappers.ServiceOrders;
using DotCruz.Garage.Domain.Entities.ServiceOrders;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Shared.Security.Context;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.ServiceOrders.Commands.CreateServiceOrder;

public class CreateServiceOrderCommandHandler : IRequestHandler<CreateServiceOrderCommand, ServiceOrderDto>
{
    private readonly IVehicleWriteRepository _vehicleWriteRepository;
    private readonly IMaintenanceScheduleWriteRepository _maintenanceScheduleWriteRepository;
    private readonly IServiceOrderWriteRepository _serviceOrderWriteRepository;
    private readonly IWorkspaceProvider _workspaceProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateServiceOrderCommandHandler(
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

    public async Task<ServiceOrderDto> Handle(CreateServiceOrderCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleWriteRepository.GetByIdWithHistoriesToUpdateAsync(request.VehicleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var workspaceId = _workspaceProvider.WorkspaceId;
        if (workspaceId.HasValue && !vehicle.CanWrite(workspaceId.Value))
        {
            throw new ForbiddenException(ResourceMessagesException.VEHICLE_ACCESS_DENIED);
        }

        var userId = _workspaceProvider.UserId ?? vehicle.UserId;

        vehicle.RecordOdometerReading(request.RequestDto.OdometerAtService, request.RequestDto.PerformedAt);

        var scheduleIds = request.RequestDto.Items
            .Where(i => i.MaintenanceScheduleId.HasValue)
            .Select(i => i.MaintenanceScheduleId!.Value)
            .Distinct()
            .ToHashSet();

        var vehicleSchedules = await _maintenanceScheduleWriteRepository.GetByVehicleIdToUpdateAsync(vehicle.Id, cancellationToken);
        if (vehicleSchedules != null)
        {
            foreach (var schedule in vehicleSchedules)
            {
                if (scheduleIds.Contains(schedule.Id))
                {
                    schedule.Perform(request.RequestDto.OdometerAtService, request.RequestDto.PerformedAt, vehicle.DailyMileageAverage);
                }
                else
                {
                    schedule.RecalculateNextDue(vehicle.CurrentOdometer, vehicle.DailyMileageAverage);
                }
            }
        }

        var serviceOrder = ServiceOrder.Create(
            workspaceId,
            userId,
            vehicle.Id,
            request.RequestDto.OrderNumber,
            request.RequestDto.WorkshopName,
            request.RequestDto.PerformedAt,
            request.RequestDto.OdometerAtService,
            request.RequestDto.TotalCost,
            request.RequestDto.Discount,
            request.RequestDto.Notes);

        foreach (var itemDto in request.RequestDto.Items)
        {
            serviceOrder.AddItem(
                itemDto.Description,
                itemDto.Type,
                itemDto.Category,
                itemDto.Quantity,
                itemDto.UnitPrice,
                itemDto.TotalCost,
                itemDto.MaintenanceScheduleId,
                itemDto.PartNumber,
                itemDto.Notes);
        }

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
