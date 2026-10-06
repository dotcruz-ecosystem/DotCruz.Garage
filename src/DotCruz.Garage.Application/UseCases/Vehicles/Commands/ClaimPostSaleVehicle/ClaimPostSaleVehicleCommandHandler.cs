using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Abstractions.Services.Security;
using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Shared.Security.Context;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.ClaimPostSaleVehicle;

public class ClaimPostSaleVehicleCommandHandler : IRequestHandler<ClaimPostSaleVehicleCommand, ClaimPostSaleVehicleResponseDto>
{
    private readonly ICheckinTokenService _tokenService;
    private readonly IVehicleSaleWriteRepository _vehicleSaleWriteRepository;
    private readonly IVehicleWriteRepository _vehicleWriteRepository;
    private readonly IMaintenanceScheduleWriteRepository _maintenanceScheduleWriteRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly IUnitOfWork _unitOfWork;

    public ClaimPostSaleVehicleCommandHandler(
        ICheckinTokenService tokenService,
        IVehicleSaleWriteRepository vehicleSaleWriteRepository,
        IVehicleWriteRepository vehicleWriteRepository,
        IMaintenanceScheduleWriteRepository maintenanceScheduleWriteRepository,
        ITenantProvider tenantProvider,
        IUnitOfWork unitOfWork)
    {
        _tokenService = tokenService;
        _vehicleSaleWriteRepository = vehicleSaleWriteRepository;
        _vehicleWriteRepository = vehicleWriteRepository;
        _maintenanceScheduleWriteRepository = maintenanceScheduleWriteRepository;
        _tenantProvider = tenantProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<ClaimPostSaleVehicleResponseDto> Handle(ClaimPostSaleVehicleCommand request, CancellationToken cancellationToken)
    {
        if (!_tokenService.TryValidatePostSaleToken(request.Token, out var portalToken) || portalToken is null)
        {
            throw new ErrorOnValidationException(ResourceMessagesException.TOKEN_INVALID);
        }

        if (portalToken.IsExpired())
        {
            throw new ErrorOnValidationException(ResourceMessagesException.TOKEN_EXPIRED);
        }

        var buyerTenantId = _tenantProvider.TenantId;
        var buyerUserId = _tenantProvider.UserId;

        if (!buyerTenantId.HasValue || !buyerUserId.HasValue)
        {
            throw new ErrorOnValidationException("Usuário deve estar autenticado em um workspace para reivindicar o veículo.");
        }

        var sale = await _vehicleSaleWriteRepository.GetSaleToUpdateUnscopedAsync(
            portalToken.SaleId,
            portalToken.TenantId,
            cancellationToken)
            ?? throw new NotFoundException("Venda do veículo não encontrada.");

        if (sale.ClaimedByUserId.HasValue)
        {
            throw new ErrorOnValidationException("Este veículo já foi reivindicado anteriormente.");
        }

        var originalVehicle = await _vehicleWriteRepository.GetByIdWithHistoriesToUpdateUnscopedAsync(
            portalToken.VehicleId,
            portalToken.TenantId,
            cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var claimedAt = DateTimeOffset.UtcNow;
        sale.MarkAsClaimed(buyerUserId.Value, claimedAt);

        var claimedVehicle = Vehicle.Create(
            buyerTenantId.Value,
            buyerUserId.Value,
            originalVehicle.Name,
            originalVehicle.Brand,
            originalVehicle.Model,
            originalVehicle.Year,
            originalVehicle.Plate,
            originalVehicle.Vin,
            originalVehicle.CurrentOdometer,
            originalVehicle.DailyMileageAverage
        );

        await _vehicleWriteRepository.AddAsync(claimedVehicle, cancellationToken);

        var originalSchedules = await _maintenanceScheduleWriteRepository.GetByVehicleIdToUpdateUnscopedAsync(
            originalVehicle.Id,
            portalToken.TenantId,
            cancellationToken);

        foreach (var schedule in originalSchedules.Where(s => s.DeletedAt == null))
        {
            var claimedSchedule = MaintenanceSchedule.Create(
                buyerTenantId.Value,
                buyerUserId.Value,
                claimedVehicle.Id,
                schedule.Description,
                schedule.Category,
                schedule.IntervalOdometer,
                schedule.IntervalTimeMonths,
                schedule.LastPerformedOdometer,
                schedule.LastPerformedDate,
                schedule.WarningThresholdOdometer,
                schedule.WarningThresholdDays,
                claimedVehicle.CurrentOdometer,
                claimedVehicle.DailyMileageAverage
            );

            await _maintenanceScheduleWriteRepository.AddAsync(claimedSchedule, cancellationToken);
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return new ClaimPostSaleVehicleResponseDto(
            claimedVehicle.Id,
            claimedVehicle.Name,
            claimedVehicle.Brand,
            claimedVehicle.Model,
            claimedVehicle.Year,
            claimedVehicle.Plate,
            claimedAt
        );
    }
}
