using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Abstractions.Services.Notifications;
using DotCruz.Garage.Application.Abstractions.Services.Security;
using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Shared.Security.Context;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.RegisterVehicleSale;

public class RegisterVehicleSaleCommandHandler : IRequestHandler<RegisterVehicleSaleCommand, RegisterVehicleSaleResponseDto>
{
    private readonly IVehicleWriteRepository _vehicleWriteRepository;
    private readonly IMaintenanceScheduleWriteRepository _maintenanceScheduleWriteRepository;
    private readonly ICheckinTokenService _tokenService;
    private readonly INotificationService _notificationService;
    private readonly IWorkspaceProvider _workspaceProvider;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterVehicleSaleCommandHandler> _logger;

    public RegisterVehicleSaleCommandHandler(
        IVehicleWriteRepository vehicleWriteRepository,
        IMaintenanceScheduleWriteRepository maintenanceScheduleWriteRepository,
        ICheckinTokenService tokenService,
        INotificationService notificationService,
        IWorkspaceProvider workspaceProvider,
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        ILogger<RegisterVehicleSaleCommandHandler> logger)
    {
        _vehicleWriteRepository = vehicleWriteRepository;
        _maintenanceScheduleWriteRepository = maintenanceScheduleWriteRepository;
        _tokenService = tokenService;
        _notificationService = notificationService;
        _workspaceProvider = workspaceProvider;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<RegisterVehicleSaleResponseDto> Handle(RegisterVehicleSaleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleWriteRepository.GetByIdWithSalesToUpdateAsync(request.VehicleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var workspaceId = _workspaceProvider.WorkspaceId ?? vehicle.WorkspaceId;
        var userId = _workspaceProvider.UserId ?? vehicle.UserId;

        var soldAt = request.SoldAt ?? DateTimeOffset.UtcNow;
        var soldOdometer = request.SoldOdometer ?? vehicle.CurrentOdometer;

        var sale = vehicle.RegisterSale(
            userId,
            request.CustomerName,
            request.CustomerEmail,
            request.CustomerPhone,
            request.CustomerDocument,
            soldAt,
            request.SoldPrice,
            soldOdometer,
            request.WarrantyExpiresAt,
            request.WarrantyKm,
            request.Notes
        );

        // Schedule post-sale maintenance routines
        var oilChangeSchedule = MaintenanceSchedule.Create(
            workspaceId,
            userId,
            vehicle.Id,
            "Troca de Óleo e Filtro (Pós-Venda)",
            MaintenanceCategory.Engine,
            intervalOdometer: 10_000,
            intervalTimeMonths: 6,
            lastPerformedOdometer: soldOdometer,
            lastPerformedDate: soldAt,
            warningThresholdOdometer: 1000,
            warningThresholdDays: 30,
            currentOdometer: vehicle.CurrentOdometer,
            dailyMileageAverage: vehicle.DailyMileageAverage
        );
        await _maintenanceScheduleWriteRepository.AddAsync(oilChangeSchedule, cancellationToken);

        if (request.WarrantyExpiresAt.HasValue || request.WarrantyKm.HasValue)
        {
            var warrantyMonths = request.WarrantyExpiresAt.HasValue
                ? Math.Max(1, (int)Math.Round((request.WarrantyExpiresAt.Value - soldAt).TotalDays / 30.0))
                : 3;

            var warrantyInspection = MaintenanceSchedule.Create(
                workspaceId,
                userId,
                vehicle.Id,
                "Revisão de Garantia da Loja",
                MaintenanceCategory.General,
                intervalOdometer: request.WarrantyKm ?? 3000,
                intervalTimeMonths: warrantyMonths,
                lastPerformedOdometer: soldOdometer,
                lastPerformedDate: soldAt,
                warningThresholdOdometer: 500,
                warningThresholdDays: 15,
                currentOdometer: vehicle.CurrentOdometer,
                dailyMileageAverage: vehicle.DailyMileageAverage
            );
            await _maintenanceScheduleWriteRepository.AddAsync(warrantyInspection, cancellationToken);
        }

        var tokenValidity = request.WarrantyExpiresAt.HasValue && request.WarrantyExpiresAt.Value > DateTimeOffset.UtcNow
            ? (request.WarrantyExpiresAt.Value - DateTimeOffset.UtcNow) + TimeSpan.FromDays(30)
            : TimeSpan.FromDays(180);

        var portalToken = _tokenService.GeneratePostSaleToken(vehicle.Id, sale.Id, workspaceId, tokenValidity);
        var baseUrl = _configuration["Settings:FrontendUrl"] ?? "https://garage.dotcruz.com";
        var portalUrl = $"{baseUrl.TrimEnd('/')}/post-sale/portal?token={portalToken}";

        await _unitOfWork.CommitAsync(cancellationToken);

        try
        {
            await _notificationService.SendPostSaleWelcomeAsync(
                userId,
                workspaceId,
                sale.CustomerName,
                sale.CustomerEmail,
                vehicle.Name,
                portalUrl,
                sale.WarrantyExpiresAt,
                sale.WarrantyKm,
                cancellationToken
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao enviar notificação de boas-vindas pós-venda para {Email}", sale.CustomerEmail);
        }

        return new RegisterVehicleSaleResponseDto(
            sale.Id,
            vehicle.Id,
            sale.CustomerName,
            sale.CustomerEmail,
            portalUrl,
            portalToken
        );
    }
}
