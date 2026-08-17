using DotCruz.Garage.Application.DTOs.Maintenance;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.CreateMaintenanceSchedule;
using DotCruz.Garage.Domain.Entities.Maintenance;

namespace DotCruz.Garage.Application.Mappers.Maintenance;

public static class MaintenanceScheduleMapper
{
    public static MaintenanceSchedule ToDomain(
        this CreateMaintenanceScheduleCommand command,
        Guid tenantId,
        Guid userId,
        int currentOdometer,
        double dailyMileageAverage)
    {
        return MaintenanceSchedule.Create(
            tenantId,
            userId,
            command.VehicleId,
            command.RequestDto.Description,
            command.RequestDto.Category,
            command.RequestDto.IntervalOdometer,
            command.RequestDto.IntervalTimeMonths,
            command.RequestDto.LastPerformedOdometer,
            command.RequestDto.LastPerformedDate,
            command.RequestDto.WarningThresholdOdometer,
            command.RequestDto.WarningThresholdDays,
            currentOdometer,
            dailyMileageAverage
        );
    }

    public static MaintenanceScheduleDto ToDto(this MaintenanceSchedule schedule)
    {
        return new MaintenanceScheduleDto(
            schedule.Id,
            schedule.VehicleId,
            schedule.Description,
            schedule.Category,
            schedule.IntervalOdometer,
            schedule.IntervalTimeMonths,
            schedule.LastPerformedOdometer,
            schedule.LastPerformedDate,
            schedule.NextDueOdometer,
            schedule.NextDueDate,
            schedule.Status.ToString(),
            schedule.WarningThresholdOdometer,
            schedule.WarningThresholdDays
        );
    }
}
