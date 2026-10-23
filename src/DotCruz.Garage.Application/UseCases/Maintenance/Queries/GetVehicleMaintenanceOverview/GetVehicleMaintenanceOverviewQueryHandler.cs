using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.ServiceOrders;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.DTOs.Maintenance;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Queries.GetVehicleMaintenanceOverview;

public class GetVehicleMaintenanceOverviewQueryHandler : IRequestHandler<GetVehicleMaintenanceOverviewQuery, MaintenanceOverviewDto>
{
    private readonly IVehicleReadRepository _vehicleReadRepository;
    private readonly IMaintenanceScheduleReadRepository _scheduleReadRepository;
    private readonly IServiceOrderReadRepository _serviceOrderReadRepository;

    public GetVehicleMaintenanceOverviewQueryHandler(
        IVehicleReadRepository vehicleReadRepository,
        IMaintenanceScheduleReadRepository scheduleReadRepository,
        IServiceOrderReadRepository serviceOrderReadRepository)
    {
        _vehicleReadRepository = vehicleReadRepository;
        _scheduleReadRepository = scheduleReadRepository;
        _serviceOrderReadRepository = serviceOrderReadRepository;
    }

    public async Task<MaintenanceOverviewDto> Handle(GetVehicleMaintenanceOverviewQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleReadRepository.GetByIdAsync(request.VehicleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var schedules = await _scheduleReadRepository.GetByVehicleIdAsync(vehicle.Id, cancellationToken);
        var serviceOrders = await _serviceOrderReadRepository.GetByVehicleIdAsync(vehicle.Id, cancellationToken);

        int totalSchedules = schedules.Count;
        int activeSchedules = schedules.Count(s => s.Status == ScheduleStatus.Active);
        int dueSoonSchedules = schedules.Count(s => s.Status == ScheduleStatus.DueSoon);
        int overdueSchedules = schedules.Count(s => s.Status == ScheduleStatus.Overdue);
        int inactiveSchedules = schedules.Count(s => s.Status == ScheduleStatus.Inactive);

        var twelveMonthsAgo = DateTimeOffset.UtcNow.AddMonths(-12);
        decimal totalCostLast12Months = serviceOrders
            .Where(so => so.PerformedAt >= twelveMonthsAgo)
            .Sum(so => so.TotalCost);

        int totalServicesPerformed = serviceOrders.Count;

        return new MaintenanceOverviewDto(
            vehicle.Id,
            totalSchedules,
            activeSchedules,
            dueSoonSchedules,
            overdueSchedules,
            inactiveSchedules,
            totalCostLast12Months,
            totalServicesPerformed);
    }
}
