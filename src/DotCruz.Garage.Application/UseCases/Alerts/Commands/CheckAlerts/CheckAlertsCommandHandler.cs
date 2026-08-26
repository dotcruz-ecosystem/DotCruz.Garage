using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Fines;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Documents;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Abstractions.Services.Auth;
using DotCruz.Garage.Application.Abstractions.Services.Notifications;
using DotCruz.Garage.Application.Abstractions.Services.Security;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Entities.Fines;
using DotCruz.Garage.Domain.Entities.Documents;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;
using DotCruz.Garage.Application.Abstractions.Services.Auth.DTOs;

namespace DotCruz.Garage.Application.UseCases.Alerts.Commands.CheckAlerts;

public class CheckAlertsCommandHandler : IRequestHandler<CheckAlertsCommand>
{
    private readonly IMaintenanceScheduleWriteRepository _scheduleWriteRepository;
    private readonly ITrafficFineWriteRepository _fineWriteRepository;
    private readonly IVehicleDocumentWriteRepository _documentWriteRepository;
    private readonly IVehicleWriteRepository? _vehicleWriteRepository;
    private readonly ICheckinTokenService? _tokenService;
    private readonly IConfiguration? _configuration;
    private readonly IAuthServiceClient _authServiceClient;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    private readonly Dictionary<(Guid UserId, Guid TenantId), UserResponseDto> _usersCache = [];

    public CheckAlertsCommandHandler(
        IMaintenanceScheduleWriteRepository scheduleWriteRepository,
        ITrafficFineWriteRepository fineWriteRepository,
        IVehicleDocumentWriteRepository documentWriteRepository,
        IAuthServiceClient authServiceClient,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        IVehicleWriteRepository? vehicleWriteRepository = null,
        ICheckinTokenService? tokenService = null,
        IConfiguration? configuration = null)
    {
        _scheduleWriteRepository = scheduleWriteRepository;
        _fineWriteRepository = fineWriteRepository;
        _documentWriteRepository = documentWriteRepository;
        _authServiceClient = authServiceClient;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _vehicleWriteRepository = vehicleWriteRepository;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task Handle(CheckAlertsCommand request, CancellationToken cancellationToken)
    {
        await ProcessMaintenanceAlertsAsync(cancellationToken);
        await ProcessTrafficFineAlertsAsync(cancellationToken);
        await ProcessDocumentAlertsAsync(cancellationToken);
        await ProcessOdometerCheckinAlertsAsync(cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task ProcessOdometerCheckinAlertsAsync(CancellationToken cancellationToken)
    {
        if (_vehicleWriteRepository is null || _tokenService is null)
            return;

        var staleVehicles = await _vehicleWriteRepository.GetVehiclesWithStaleOdometerAsync(15, cancellationToken);

        foreach (var vehicle in staleVehicles)
        {
            var user = await GetUserAsync(vehicle.UserId, vehicle.TenantId, cancellationToken);
            if (user is null)
                continue;

            var validity = TimeSpan.FromHours(48);
            var token = _tokenService.GenerateToken(vehicle.Id, vehicle.TenantId, validity);
            var baseUrl = _configuration?["Settings:FrontendUrl"] ?? "https://garage.dotcruz.com";
            var magicLinkUrl = $"{baseUrl.TrimEnd('/')}/checkin/odometer?token={token}";

            await _notificationService.SendOdometerCheckinRequestAsync(
                userId: vehicle.UserId,
                tenantId: vehicle.TenantId,
                userName: user.Name,
                userEmail: user.Email,
                vehicleName: vehicle.Name,
                currentOdometer: vehicle.CurrentOdometer,
                magicLinkUrl: magicLinkUrl,
                expiresAt: DateTimeOffset.UtcNow.Add(validity),
                cancellationToken: cancellationToken);
        }
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
        var user = await GetUserAsync(maintenance.UserId, maintenance.TenantId, cancellationToken);
        if (user == null)
            return;

        await _notificationService.SendMaintenanceAlertAsync(
            userId: maintenance.UserId,
            tenantId: maintenance.TenantId,
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

    private async Task ProcessTrafficFineAlertsAsync(CancellationToken cancellationToken)
    {
        var fines = await _fineWriteRepository.GetPendingFinesToNotifyAsync(cancellationToken);

        foreach (var fine in fines)
        {
            await NotifyTrafficFineAsync(fine, cancellationToken);
        }
    }

    private async Task NotifyTrafficFineAsync(TrafficFine fine, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(fine.UserId, fine.TenantId, cancellationToken);
        if (user == null)
            return;

        await _notificationService.SendTrafficFineAlertAsync(
            userId: fine.UserId,
            tenantId: fine.TenantId,
            userName: user.Name,
            userEmail: user.Email,
            vehicleName: fine.Vehicle.Name,
            infractionDescription: fine.Description,
            amount: fine.Amount,
            dueDate: fine.DueDate,
            cancellationToken: cancellationToken
        );

        fine.Notify();
    }

    private async Task ProcessDocumentAlertsAsync(CancellationToken cancellationToken)
    {
        var documents = await _documentWriteRepository.GetDocumentsToNotifyAsync(cancellationToken);

        foreach (var doc in documents)
        {
            await NotifyDocumentAsync(doc, cancellationToken);
        }
    }

    private async Task NotifyDocumentAsync(VehicleDocument doc, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(doc.UserId, doc.TenantId, cancellationToken);
        if (user == null)
            return;

        var alertType = doc.Status == DocumentStatus.Pending ? "PaymentPending" : "Expiring";
        var statusToNotify = doc.Status == DocumentStatus.Pending ? DocumentStatus.Pending : DocumentStatus.Expired;

        await _notificationService.SendDocumentAlertAsync(
            userId: doc.UserId,
            tenantId: doc.TenantId,
            userName: user.Name,
            userEmail: user.Email,
            vehicleName: doc.Vehicle.Name,
            documentType: doc.Type.ToString(),
            alertType: alertType,
            dueDate: doc.DueDate,
            expirationDate: doc.ExpirationDate,
            cancellationToken: cancellationToken
        );

        doc.Notify(statusToNotify);
    }

    private async Task<UserResponseDto?> GetUserAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken)
    {
        var key = (userId, tenantId);

        if (_usersCache.TryGetValue(key, out var cached))
            return cached;

        var user = await _authServiceClient.GetUserByIdAsync(userId, tenantId, cancellationToken);

        if (user != null)
            _usersCache[key] = user;

        return user;
    }
}
