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
            originWorkspaceId: Guid.NewGuid(),
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
            originWorkspaceId: Guid.NewGuid(),
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
            originWorkspaceId: Guid.NewGuid(),
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
            originWorkspaceId: Guid.NewGuid(),
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

    [Fact]
    public void Status_Is_PendingFirstExecution_When_Created_Without_Previous_Performance()
    {
        var schedule = MaintenanceSchedule.Create(
            originWorkspaceId: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            vehicleId: Guid.NewGuid(),
            description: "Troca de Óleo",
            category: MaintenanceCategory.Engine,
            intervalOdometer: 10_000,
            intervalTimeMonths: 6,
            lastPerformedOdometer: null,
            lastPerformedDate: null,
            warningThresholdOdometer: 1_000,
            warningThresholdDays: 15,
            currentOdometer: 52_000,
            dailyMileageAverage: 20);

        Assert.Equal(ScheduleStatus.PendingFirstExecution, schedule.Status);
        Assert.Null(schedule.NextDueOdometer);
        Assert.Null(schedule.NextDueDate);

        schedule.Perform(52_000, DateTimeOffset.UtcNow, 20);

        Assert.Equal(ScheduleStatus.Active, schedule.Status);
        Assert.Equal(62_000, schedule.NextDueOdometer);
    }

    [Fact]
    public void Schedule_With_Odometer_Only_Interval_Has_No_NextDueDate_And_Only_Overdues_By_Km()
    {
        var schedule = MaintenanceSchedule.Create(
            originWorkspaceId: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            vehicleId: Guid.NewGuid(),
            description: "Velas de Ignição da Moto",
            category: MaintenanceCategory.Engine,
            intervalOdometer: 10_000,
            intervalTimeMonths: null,
            lastPerformedOdometer: 20_000,
            lastPerformedDate: DateTimeOffset.UtcNow.AddYears(-2), // 2 years ago, but bike not ridden
            warningThresholdOdometer: 1_000,
            warningThresholdDays: null,
            currentOdometer: 25_000,
            dailyMileageAverage: 20);

        Assert.Equal(30_000, schedule.NextDueOdometer);
        Assert.Null(schedule.NextDueDate); // No date due!
        Assert.Equal(ScheduleStatus.Active, schedule.Status); // Not overdue even though 2 years passed!

        // Now advance current odometer past 30,000
        schedule.RecalculateNextDue(30_500, 20);
        Assert.Equal(ScheduleStatus.Overdue, schedule.Status);
    }

    [Fact]
    public void Schedule_With_Time_Only_Interval_Has_No_NextDueOdometer_And_Only_Overdues_By_Date()
    {
        var schedule = MaintenanceSchedule.Create(
            originWorkspaceId: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            vehicleId: Guid.NewGuid(),
            description: "Fluido de Freio",
            category: MaintenanceCategory.Brakes,
            intervalOdometer: null,
            intervalTimeMonths: 12,
            lastPerformedOdometer: null,
            lastPerformedDate: DateTimeOffset.UtcNow.AddMonths(-14),
            warningThresholdOdometer: null,
            warningThresholdDays: 15,
            currentOdometer: 80_000,
            dailyMileageAverage: 20);

        Assert.Null(schedule.NextDueOdometer); // No km due!
        Assert.NotNull(schedule.NextDueDate);
        Assert.Equal(ScheduleStatus.Overdue, schedule.Status);
    }

    [Fact]
    public void Gradual_Checkpoints_Advance_Sequentially_Until_Steady_State()
    {
        // MT-03 2023 case: 1.000km (6m) -> 5.000km (12m) -> 10.000km (18m) -> steady-state +5.000km (6m)
        var schedule = MaintenanceSchedule.Create(
            originWorkspaceId: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            vehicleId: Guid.NewGuid(),
            description: "Troca do Óleo do Motor",
            category: MaintenanceCategory.Engine,
            intervalOdometer: 5_000,
            intervalTimeMonths: 6,
            lastPerformedOdometer: 0,
            lastPerformedDate: DateTimeOffset.UtcNow,
            warningThresholdOdometer: 500,
            warningThresholdDays: 15,
            currentOdometer: 0,
            dailyMileageAverage: 20);

        schedule.AddCheckpoint(1_000, 6, 1);
        schedule.AddCheckpoint(5_000, 12, 2);
        schedule.AddCheckpoint(10_000, 18, 3);

        schedule.RecalculateNextDue(0, 20);

        // 1st due: 1.000 km
        Assert.Equal(1_000, schedule.NextDueOdometer);

        // Perform 1st service at 1.000 km
        schedule.Perform(1_000, DateTimeOffset.UtcNow, 20);
        Assert.True(schedule.Checkpoints.First(c => c.Odometer == 1_000).IsCompleted);
        // Next due should be 2nd checkpoint: 5.000 km
        Assert.Equal(5_000, schedule.NextDueOdometer);

        // Perform 2nd service at 5.000 km
        schedule.Perform(5_000, DateTimeOffset.UtcNow, 20);
        Assert.True(schedule.Checkpoints.First(c => c.Odometer == 5_000).IsCompleted);
        // Next due should be 3rd checkpoint: 10.000 km
        Assert.Equal(10_000, schedule.NextDueOdometer);

        // Perform 3rd service at 10.000 km (end of ramp-up)
        schedule.Perform(10_000, DateTimeOffset.UtcNow, 20);
        Assert.True(schedule.Checkpoints.First(c => c.Odometer == 10_000).IsCompleted);
        // All checkpoints completed! Enters steady-state (+5.000 km): 15.000 km
        Assert.Equal(15_000, schedule.NextDueOdometer);

        // Subsequent performance stays in steady-state (+5.000 km)
        schedule.Perform(15_000, DateTimeOffset.UtcNow, 20);
        Assert.Equal(20_000, schedule.NextDueOdometer);
    }

    [Fact]
    public void Checkpoints_Superceded_When_Vehicle_Starts_Used()
    {
        // Vehicle enters at 8.000 km. Checkpoints 1.000 and 5.000 are completed/superceded.
        var schedule = MaintenanceSchedule.Create(
            originWorkspaceId: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            vehicleId: Guid.NewGuid(),
            description: "Troca do Óleo do Motor",
            category: MaintenanceCategory.Engine,
            intervalOdometer: 5_000,
            intervalTimeMonths: 6,
            lastPerformedOdometer: 0,
            lastPerformedDate: DateTimeOffset.UtcNow,
            warningThresholdOdometer: 500,
            warningThresholdDays: 15,
            currentOdometer: 8_000,
            dailyMileageAverage: 20);

        schedule.AddCheckpoint(1_000, 6, 1, isCompleted: true, completedAt: DateTimeOffset.UtcNow, completedOdometer: 1_000);
        schedule.AddCheckpoint(5_000, 12, 2, isCompleted: true, completedAt: DateTimeOffset.UtcNow, completedOdometer: 5_000);
        schedule.AddCheckpoint(10_000, 18, 3, isCompleted: false);

        schedule.RecalculateNextDue(8_000, 20);

        // Next due should target the uncompleted 10.000 km checkpoint!
        Assert.Equal(10_000, schedule.NextDueOdometer);
    }
}
