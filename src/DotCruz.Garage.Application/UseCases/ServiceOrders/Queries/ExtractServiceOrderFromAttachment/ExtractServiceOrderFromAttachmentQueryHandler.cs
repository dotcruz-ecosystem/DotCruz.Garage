using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Abstractions.Services.Ai;
using DotCruz.Garage.Application.DTOs.ServiceOrders;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Shared.Security.Context;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.ServiceOrders.Queries.ExtractServiceOrderFromAttachment;

public class ExtractServiceOrderFromAttachmentQueryHandler : IRequestHandler<ExtractServiceOrderFromAttachmentQuery, ExtractedServiceOrderDto>
{
    private readonly IVehicleReadRepository _vehicleReadRepository;
    private readonly IMaintenanceScheduleReadRepository _maintenanceScheduleReadRepository;
    private readonly IAiServiceOrderExtractionService _aiServiceOrderExtractionService;
    private readonly IWorkspaceProvider _workspaceProvider;

    public ExtractServiceOrderFromAttachmentQueryHandler(
        IVehicleReadRepository vehicleReadRepository,
        IMaintenanceScheduleReadRepository maintenanceScheduleReadRepository,
        IAiServiceOrderExtractionService aiServiceOrderExtractionService,
        IWorkspaceProvider workspaceProvider)
    {
        _vehicleReadRepository = vehicleReadRepository;
        _maintenanceScheduleReadRepository = maintenanceScheduleReadRepository;
        _aiServiceOrderExtractionService = aiServiceOrderExtractionService;
        _workspaceProvider = workspaceProvider;
    }

    public async Task<ExtractedServiceOrderDto> Handle(ExtractServiceOrderFromAttachmentQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleReadRepository.GetByIdAsync(request.VehicleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var workspaceId = _workspaceProvider.WorkspaceId;
        if (workspaceId.HasValue && !vehicle.CanWrite(workspaceId.Value))
        {
            throw new ForbiddenException(ResourceMessagesException.VEHICLE_ACCESS_DENIED);
        }

        var schedules = await _maintenanceScheduleReadRepository.GetByVehicleIdAsync(vehicle.Id, cancellationToken);
        var activeSchedules = schedules
            .Where(s => s.Status == ScheduleStatus.Active || s.Status == ScheduleStatus.DueSoon || s.Status == ScheduleStatus.Overdue)
            .ToList();

        var extracted = await _aiServiceOrderExtractionService.ExtractServiceOrderAsync(
            request.RequestDto.FileUrl,
            request.RequestDto.FileName,
            request.RequestDto.ContentType,
            activeSchedules,
            cancellationToken);

        return extracted;
    }
}
