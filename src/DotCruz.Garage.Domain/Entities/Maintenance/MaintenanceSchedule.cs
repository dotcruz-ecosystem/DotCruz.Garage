using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Domain.Entities.Maintenance;

public class MaintenanceSchedule : BaseEntity
{
    public const int DefaultWarningThresholdOdometer = 1000;
    public const int DefaultWarningThresholdDays = 30;

    public Guid? OriginWorkspaceId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid VehicleId { get; private set; }
    public string Description { get; private set; } = null!;
    public MaintenanceCategory Category { get; private set; }
    public int? IntervalOdometer { get; private set; }
    public int? IntervalTimeMonths { get; private set; }
    public int? LastPerformedOdometer { get; private set; }
    public DateTimeOffset? LastPerformedDate { get; private set; }
    public int? NextDueOdometer { get; private set; }
    public DateTimeOffset? NextDueDate { get; private set; }
    public DateTimeOffset? LastNotifiedAt { get; private set; }
    public ScheduleStatus? LastNotifiedStatus { get; private set; }
    public ScheduleStatus Status { get; private set; }
    public int? WarningThresholdOdometer { get; private set; }
    public int? WarningThresholdDays { get; private set; }

    public virtual ICollection<MaintenanceScheduleCheckpoint> Checkpoints { get; private init; } = new List<MaintenanceScheduleCheckpoint>();
    public virtual Vehicle Vehicle { get; private init; } = default!;

    private MaintenanceSchedule() { }

    public static MaintenanceSchedule Create(
        Guid? originWorkspaceId,
        Guid userId,
        Guid vehicleId,
        string description,
        MaintenanceCategory category,
        int? intervalOdometer,
        int? intervalTimeMonths,
        int? lastPerformedOdometer,
        DateTimeOffset? lastPerformedDate,
        int? warningThresholdOdometer,
        int? warningThresholdDays,
        int currentOdometer,
        double dailyMileageAverage)
    {
        var schedule = new MaintenanceSchedule
        {
            OriginWorkspaceId = originWorkspaceId,
            UserId = userId,
            VehicleId = vehicleId,
            Description = description,
            Category = category,
            IntervalOdometer = intervalOdometer,
            IntervalTimeMonths = intervalTimeMonths,
            LastPerformedOdometer = lastPerformedOdometer,
            LastPerformedDate = lastPerformedDate,
            WarningThresholdOdometer = warningThresholdOdometer,
            WarningThresholdDays = warningThresholdDays,
            Status = ScheduleStatus.Active
        };

        if (!lastPerformedOdometer.HasValue && !lastPerformedDate.HasValue)
        {
            schedule.Status = ScheduleStatus.PendingFirstExecution;
            schedule.NextDueOdometer = null;
            schedule.NextDueDate = null;
            return schedule;
        }

        schedule.RecalculateNextDue(currentOdometer, dailyMileageAverage);

        return schedule;
    }

    public void AddCheckpoint(
        int odometer,
        int months,
        int? order = null,
        bool isCompleted = false,
        DateTimeOffset? completedAt = null,
        int? completedOdometer = null)
    {
        var nextOrder = order ?? (Checkpoints.Count + 1);
        Checkpoints.Add(MaintenanceScheduleCheckpoint.Create(
            Id,
            odometer,
            months,
            nextOrder,
            isCompleted,
            completedAt,
            completedOdometer));
    }

    public void RecalculateNextDue(int currentOdometer, double dailyMileageAverage)
    {
        if (!LastPerformedOdometer.HasValue && !LastPerformedDate.HasValue)
        {
            Status = ScheduleStatus.PendingFirstExecution;
            NextDueOdometer = null;
            NextDueDate = null;
            return;
        }

        DateTimeOffset baseDate = LastPerformedDate ?? CreatedAt;
        DateTimeOffset? timeBasedDate = null;

        var nextCheckpoint = Checkpoints
            .Where(c => !c.IsCompleted && c.Odometer > (LastPerformedOdometer ?? 0))
            .OrderBy(c => c.Order)
            .ThenBy(c => c.Odometer)
            .FirstOrDefault();

        if (nextCheckpoint != null)
        {
            NextDueOdometer = nextCheckpoint.Odometer;
            var originDate = Vehicle?.CreatedAt ?? CreatedAt;
            timeBasedDate = originDate.AddMonths(nextCheckpoint.Months);
            if (timeBasedDate < baseDate && IntervalTimeMonths.HasValue)
            {
                timeBasedDate = baseDate.AddMonths(IntervalTimeMonths.Value);
            }
        }
        else
        {
            int baseOdometer = LastPerformedOdometer ?? currentOdometer;
            NextDueOdometer = IntervalOdometer.HasValue ? baseOdometer + IntervalOdometer.Value : null;

            if (IntervalTimeMonths.HasValue)
                timeBasedDate = baseDate.AddMonths(IntervalTimeMonths.Value);
        }

        DateTimeOffset? mileageBasedDate = null;

        if (NextDueOdometer.HasValue && dailyMileageAverage > 0)
        {
            int remainingDistance = NextDueOdometer.Value - currentOdometer;
            if (remainingDistance > 0)
            {
                double daysUntilNextDue = remainingDistance / dailyMileageAverage;
                mileageBasedDate = DateTimeOffset.UtcNow.AddDays(daysUntilNextDue);
            }
            else
            {
                mileageBasedDate = DateTimeOffset.UtcNow;
            }
        }

        if (timeBasedDate.HasValue && mileageBasedDate.HasValue)
        {
            NextDueDate = timeBasedDate.Value < mileageBasedDate.Value ? timeBasedDate.Value : mileageBasedDate.Value;
        }
        else if (timeBasedDate.HasValue)
        {
            NextDueDate = timeBasedDate.Value;
        }
        else
        {
            NextDueDate = null;
        }

        UpdateStatus(currentOdometer);
    }

    public void UpdateStatus(int currentOdometer)
    {
        if (!LastPerformedOdometer.HasValue && !LastPerformedDate.HasValue)
        {
            Status = ScheduleStatus.PendingFirstExecution;
            return;
        }

        bool isOverdueByOdometer = NextDueOdometer.HasValue && currentOdometer >= NextDueOdometer.Value;
        bool isOverdueByDate = NextDueDate.HasValue && DateTimeOffset.UtcNow >= NextDueDate.Value;

        if (isOverdueByOdometer || isOverdueByDate)
        {
            Status = ScheduleStatus.Overdue;
            return;
        }

        bool isSoonByOdometer = NextDueOdometer.HasValue && (NextDueOdometer.Value - currentOdometer) <= (WarningThresholdOdometer ?? DefaultWarningThresholdOdometer);
        bool isSoonByDate = NextDueDate.HasValue && (NextDueDate.Value - DateTimeOffset.UtcNow).TotalDays <= (WarningThresholdDays ?? DefaultWarningThresholdDays);

        if (isSoonByOdometer || isSoonByDate)
        {
            Status = ScheduleStatus.DueSoon;
            return;
        }

        Status = ScheduleStatus.Active;
    }

    public void Perform(int odometerAtService, DateTimeOffset dateOfService, double dailyMileageAverage)
    {
        LastPerformedOdometer = odometerAtService;
        LastPerformedDate = dateOfService;
        LastNotifiedAt = null;
        LastNotifiedStatus = null;

        foreach (var cp in Checkpoints.Where(c => !c.IsCompleted && c.Odometer <= odometerAtService))
        {
            cp.Complete(odometerAtService, dateOfService);
        }

        RecalculateNextDue(odometerAtService, dailyMileageAverage);
    }

    public void Update(
        string description,
        MaintenanceCategory category,
        int? intervalOdometer,
        int? intervalTimeMonths,
        int? warningThresholdOdometer,
        int? warningThresholdDays,
        int currentOdometer,
        double dailyMileageAverage)
    {
        Description = description;
        Category = category;
        IntervalOdometer = intervalOdometer;
        IntervalTimeMonths = intervalTimeMonths;
        WarningThresholdOdometer = warningThresholdOdometer;
        WarningThresholdDays = warningThresholdDays;
        RecalculateNextDue(currentOdometer, dailyMileageAverage);
    }

    public void Notify(ScheduleStatus status)
    {
        LastNotifiedAt = DateTimeOffset.UtcNow;
        LastNotifiedStatus = status;
    }
}
