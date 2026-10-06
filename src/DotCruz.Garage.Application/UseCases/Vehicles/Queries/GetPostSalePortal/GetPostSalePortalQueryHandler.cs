using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Abstractions.Services.Security;
using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetPostSalePortal;

public class GetPostSalePortalQueryHandler : IRequestHandler<GetPostSalePortalQuery, PostSalePortalDto>
{
    private readonly ICheckinTokenService _tokenService;
    private readonly IVehicleSaleReadRepository _vehicleSaleReadRepository;
    private readonly IVehicleWriteRepository _vehicleWriteRepository;
    private readonly IMaintenanceScheduleWriteRepository _maintenanceScheduleWriteRepository;
    private readonly IMaintenanceLogReadRepository _maintenanceLogReadRepository;

    public GetPostSalePortalQueryHandler(
        ICheckinTokenService tokenService,
        IVehicleSaleReadRepository vehicleSaleReadRepository,
        IVehicleWriteRepository vehicleWriteRepository,
        IMaintenanceScheduleWriteRepository maintenanceScheduleWriteRepository,
        IMaintenanceLogReadRepository maintenanceLogReadRepository)
    {
        _tokenService = tokenService;
        _vehicleSaleReadRepository = vehicleSaleReadRepository;
        _vehicleWriteRepository = vehicleWriteRepository;
        _maintenanceScheduleWriteRepository = maintenanceScheduleWriteRepository;
        _maintenanceLogReadRepository = maintenanceLogReadRepository;
    }

    public async Task<PostSalePortalDto> Handle(GetPostSalePortalQuery request, CancellationToken cancellationToken)
    {
        if (!_tokenService.TryValidatePostSaleToken(request.Token, out var portalToken) || portalToken is null)
        {
            throw new ErrorOnValidationException(ResourceMessagesException.TOKEN_INVALID);
        }

        if (portalToken.IsExpired())
        {
            throw new ErrorOnValidationException(ResourceMessagesException.TOKEN_EXPIRED);
        }

        var sale = await _vehicleSaleReadRepository.GetSaleUnscopedAsync(portalToken.SaleId, portalToken.TenantId, cancellationToken)
            ?? throw new NotFoundException("Venda do veículo não encontrada.");

        var vehicle = sale.Vehicle 
            ?? await _vehicleWriteRepository.GetByIdWithHistoriesToUpdateUnscopedAsync(portalToken.VehicleId, portalToken.TenantId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var schedules = await _maintenanceScheduleWriteRepository.GetByVehicleIdToUpdateUnscopedAsync(
            vehicle.Id,
            portalToken.TenantId,
            cancellationToken);

        var logs = await _maintenanceLogReadRepository.GetByVehicleIdUnscopedAsync(
            vehicle.Id,
            portalToken.TenantId,
            cancellationToken);

        var now = DateTimeOffset.UtcNow;

        var upcomingMaintenance = schedules
            .Where(s => s.DeletedAt == null)
            .OrderBy(s => s.NextDueDate ?? DateTimeOffset.MaxValue)
            .Select(s => new PostSaleMaintenanceScheduleDto(
                s.Id,
                s.Description,
                s.Category.ToString(),
                s.NextDueOdometer,
                s.NextDueDate,
                s.Status.ToString()))
            .ToList();

        var maintenanceHistory = logs
            .Where(l => l.DeletedAt == null)
            .OrderByDescending(l => l.PerformedAt)
            .Select(l => new PostSaleMaintenanceLogDto(
                l.Id,
                l.Description,
                l.PerformedAt,
                l.OdometerAtService,
                l.Cost))
            .ToList();

        return new PostSalePortalDto(
            vehicle.Id,
            vehicle.Name,
            vehicle.Brand,
            vehicle.Model,
            vehicle.Year,
            vehicle.Plate,
            vehicle.CurrentOdometer,
            sale.CustomerName,
            sale.SoldAt,
            sale.WarrantyExpiresAt,
            sale.WarrantyKm,
            sale.IsWarrantyActive(vehicle.CurrentOdometer, now),
            sale.RemainingWarrantyDays(now),
            sale.RemainingWarrantyKm(vehicle.CurrentOdometer),
            upcomingMaintenance,
            maintenanceHistory
        );
    }
}
