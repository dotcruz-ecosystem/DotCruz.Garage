using CommonTestUtilities.Entities;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Enums;

namespace UseCases.Test.Maintenance;

public class MaintenanceScheduleDomainTest
{
    [Fact]
    public void Status_Is_Active_When_Within_Safe_Range()
    {
        var schedule = MaintenanceSchedule.Create(
            tenantId: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            vehicleId: Guid.NewGuid(),
            description: "Troca de Óleo",
            category: MaintenanceCategory.Engine,
            intervalOdometer: 10_000,
            intervalTimeMonths: 6,
            lastPerformedOdometer: 50_000,
            lastPerformedDate: DateTimeOffset.UtcNow,
            warningThresholdOdometer: 1_000,
            warningThresholdDays: 15,
            currentOdometer: 52_000,
            dailyMileageAverage: 20);

        Assert.Equal(ScheduleStatus.Active, schedule.Status);
        Assert.Equal(60_000, schedule.NextDueOdometer);
    }

    [Fact]
    public void Status_Changes_To_DueSoon_When_Reaching_Warning_Threshold_Odometer()
    {
        var schedule = MaintenanceSchedule.Create(
            tenantId: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            vehicleId: Guid.NewGuid(),
            description: "Troca de Óleo",
            category: MaintenanceCategory.Engine,
            intervalOdometer: 10_000,
            intervalTimeMonths: 6,
            lastPerformedOdometer: 50_000,
            lastPerformedDate: DateTimeOffset.UtcNow,
            warningThresholdOdometer: 1_000,
            warningThresholdDays: 15,
            currentOdometer: 59_200, // 800 km away <= 1,000 threshold
            dailyMileageAverage: 20);

        Assert.Equal(ScheduleStatus.DueSoon, schedule.Status);
    }

    [Fact]
    public void Status_Changes_To_Overdue_When_Odometer_Exceeds_NextDue()
    {
        var schedule = MaintenanceSchedule.Create(
            tenantId: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            vehicleId: Guid.NewGuid(),
            description: "Troca de Óleo",
            category: MaintenanceCategory.Engine,
            intervalOdometer: 10_000,
            intervalTimeMonths: 6,
            lastPerformedOdometer: 50_000,
            lastPerformedDate: DateTimeOffset.UtcNow,
            warningThresholdOdometer: 1_000,
            warningThresholdDays: 15,
            currentOdometer: 60_500, // Exceeded 60,000 NextDue
            dailyMileageAverage: 20);

        Assert.Equal(ScheduleStatus.Overdue, schedule.Status);
    }

    [Fact]
    public void Perform_Advances_NextDue_To_Following_Cycle()
    {
        var schedule = MaintenanceSchedule.Create(
            tenantId: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            vehicleId: Guid.NewGuid(),
            description: "Revisão Geral",
            category: MaintenanceCategory.General,
            intervalOdometer: 10_000,
            intervalTimeMonths: 6,
            lastPerformedOdometer: 50_000,
            lastPerformedDate: DateTimeOffset.UtcNow.AddMonths(-6),
            warningThresholdOdometer: 1_000,
            warningThresholdDays: 15,
            currentOdometer: 60_100,
            dailyMileageAverage: 20);

        Assert.Equal(ScheduleStatus.Overdue, schedule.Status);

        schedule.Perform(60_100, DateTimeOffset.UtcNow, 20);

        Assert.Equal(60_100, schedule.LastPerformedOdometer);
        Assert.Equal(70_100, schedule.NextDueOdometer);
        Assert.Equal(ScheduleStatus.Active, schedule.Status);
        Assert.Null(schedule.LastNotifiedAt);
        Assert.Null(schedule.LastNotifiedStatus);
    }
}
