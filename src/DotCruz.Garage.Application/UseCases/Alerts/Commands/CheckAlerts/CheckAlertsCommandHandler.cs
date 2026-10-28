using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Services.Auth;
using DotCruz.Garage.Application.Abstractions.Services.Notifications;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Enums;
using MediatR;
using DotCruz.Garage.Application.Abstractions.Services.Auth.DTOs;

namespace DotCruz.Garage.Application.UseCases.Alerts.Commands.CheckAlerts;

public class CheckAlertsCommandHandler : IRequestHandler<CheckAlertsCommand>
{
    private readonly IMaintenanceScheduleWriteRepository _scheduleWriteRepository;
    private readonly IAuthServiceClient _authServiceClient;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    private readonly Dictionary<(Guid UserId, Guid WorkspaceId), UserResponseDto> _usersCache = [];

    public CheckAlertsCommandHandler(
        IMaintenanceScheduleWriteRepository scheduleWriteRepository,
        IAuthServiceClient authServiceClient,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _scheduleWriteRepository = scheduleWriteRepository;
        _authServiceClient = authServiceClient;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CheckAlertsCommand request, CancellationToken cancellationToken)
    {
        await ProcessMaintenanceAlertsAsync(cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task ProcessMaintenanceAlertsAsync(CancellationToken cancellationToken)
    {
        var maintenancesSchedule = await _scheduleWriteRepository.GetSchedulesWithVehicleDetailsToUpdateAsync(cancellationToken);

        foreach (var maintenance in maintenancesSchedule)
        {
            maintenance.UpdateStatus(maintenance.Vehicle.CurrentOdometer);
            var status = maintenance.Status;

            if (maintenance.Status == ScheduleStatus.Active || maintenance.Status == ScheduleStatus.Inactive || maintenance.LastNotifiedStatus == maintenance.Status)
                continue;

            await NotifyMaintenanceAsync(maintenance, status, cancellationToken);
        }
    }

    private async Task NotifyMaintenanceAsync(MaintenanceSchedule maintenance, ScheduleStatus status, CancellationToken cancellationToken)
    {
        var workspaceId = maintenance.OriginWorkspaceId 
            ?? maintenance.Vehicle.Workspaces.FirstOrDefault(w => w.Role == Domain.Enums.VehicleWorkspaceRole.Owner)?.WorkspaceId 
            ?? Guid.Empty;

        var user = await GetUserAsync(maintenance.UserId, workspaceId, cancellationToken);
        if (user == null)
            return;

        await _notificationService.SendMaintenanceAlertAsync(
            userId: maintenance.UserId,
            workspaceId: workspaceId,
            userName: user.Name,
            userEmail: user.Email,
            vehicleName: maintenance.Vehicle.Name,
            maintenanceDescription: maintenance.Description,
            dueOdometer: maintenance.NextDueOdometer,
            dueDate: maintenance.NextDueDate,
            cancellationToken: cancellationToken
        );

        maintenance.Notify(status);
    }

    private async Task<UserResponseDto?> GetUserAsync(Guid userId, Guid workspaceId, CancellationToken cancellationToken)
    {
        var key = (userId, workspaceId);

        if (_usersCache.TryGetValue(key, out var cached))
            return cached;

        var user = await _authServiceClient.GetUserByIdAsync(userId, workspaceId, cancellationToken);

        if (user != null)
            _usersCache[key] = user;

        return user;
    }
}
