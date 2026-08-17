using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Fines;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Documents;
using DotCruz.Garage.Application.Abstractions.Services.Auth;
using DotCruz.Garage.Application.Abstractions.Services.Notifications;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Entities.Fines;
using DotCruz.Garage.Domain.Entities.Documents;
using DotCruz.Garage.Domain.Enums;
using MediatR;
using System.Collections.Concurrent;
using DotCruz.Garage.Application.Abstractions.Services.Auth.DTOs;

namespace DotCruz.Garage.Application.UseCases.Alerts.Commands.CheckAlerts;

public class CheckAlertsCommandHandler : IRequestHandler<CheckAlertsCommand>
{
    private readonly IMaintenanceScheduleWriteRepository _scheduleWriteRepository;
    private readonly ITrafficFineWriteRepository _fineWriteRepository;
    private readonly IVehicleDocumentWriteRepository _documentWriteRepository;
    private readonly IAuthServiceClient _authServiceClient;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    private readonly ConcurrentDictionary<Guid, UserResponseDto> _usersDictionary = new();

    public CheckAlertsCommandHandler(
        IMaintenanceScheduleWriteRepository scheduleWriteRepository,
        ITrafficFineWriteRepository fineWriteRepository,
        IVehicleDocumentWriteRepository documentWriteRepository,
        IAuthServiceClient authServiceClient,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _scheduleWriteRepository = scheduleWriteRepository;
        _fineWriteRepository = fineWriteRepository;
        _documentWriteRepository = documentWriteRepository;
        _authServiceClient = authServiceClient;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CheckAlertsCommand request, CancellationToken cancellationToken)
    {
        await ProcessMaintenanceAlertsAsync(cancellationToken);
        await ProcessTrafficFineAlertsAsync(cancellationToken);
        await ProcessDocumentAlertsAsync(cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task ProcessMaintenanceAlertsAsync(CancellationToken cancellationToken)
    {
        var maintenancesSchedule = await _scheduleWriteRepository.GetSchedulesWithVehicleDetailsToUpdateAsync(cancellationToken);
        var tasks = new List<Task>();

        foreach (var maintenance in maintenancesSchedule)
        {
            maintenance.UpdateStatus(maintenance.Vehicle.CurrentOdometer);
            var status = maintenance.Status;

            if (maintenance.Status == ScheduleStatus.Active || maintenance.Status == ScheduleStatus.Inactive || maintenance.LastNotifiedStatus == maintenance.Status)
                continue;

            tasks.Add(NotifyMaintenanceAsync(maintenance, status, cancellationToken));
        }

        await Task.WhenAll(tasks);
    }

    private async Task NotifyMaintenanceAsync(MaintenanceSchedule maintenance, ScheduleStatus status, CancellationToken cancellationToken)
    {
        var user = await GetOrCreateUserAsync(maintenance.UserId, cancellationToken);
        if (user == null)
            return;

        await _notificationService.SendMaintenanceAlertAsync(
            userId: maintenance.UserId,
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
        var tasks = new List<Task>();

        foreach (var fine in fines)
        {
            tasks.Add(NotifyTrafficFineAsync(fine, cancellationToken));
        }

        await Task.WhenAll(tasks);
    }

    private async Task NotifyTrafficFineAsync(TrafficFine fine, CancellationToken cancellationToken)
    {
        var user = await GetOrCreateUserAsync(fine.UserId, cancellationToken);
        if (user == null)
            return;

        await _notificationService.SendTrafficFineAlertAsync(
            userId: fine.UserId,
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
        var tasks = new List<Task>();

        foreach (var doc in documents)
        {
            tasks.Add(NotifyDocumentAsync(doc, cancellationToken));
        }

        await Task.WhenAll(tasks);
    }

    private async Task NotifyDocumentAsync(VehicleDocument doc, CancellationToken cancellationToken)
    {
        var user = await GetOrCreateUserAsync(doc.UserId, cancellationToken);
        if (user == null)
            return;

        var alertType = doc.Status == DocumentStatus.Pending ? "PaymentPending" : "Expiring";
        var statusToNotify = doc.Status == DocumentStatus.Pending ? DocumentStatus.Pending : DocumentStatus.Expired;

        await _notificationService.SendDocumentAlertAsync(
            userId: doc.UserId,
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

    private async Task<UserResponseDto?> GetOrCreateUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        if (_usersDictionary.TryGetValue(userId, out var user))
            return user;

        user = await _authServiceClient.GetUserByIdAsync(userId, cancellationToken);
        if (user != null)
            _usersDictionary.TryAdd(userId, user);

        return user;
    }
}
