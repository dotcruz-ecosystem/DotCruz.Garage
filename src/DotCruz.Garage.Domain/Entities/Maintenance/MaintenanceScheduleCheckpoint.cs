using DotCruz.Garage.Domain.Entities.Base;

namespace DotCruz.Garage.Domain.Entities.Maintenance;

public class MaintenanceScheduleCheckpoint : BaseEntity
{
    public Guid ScheduleId { get; private set; }
    public int Odometer { get; private set; }
    public int Months { get; private set; }
    public int Order { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public int? CompletedOdometer { get; private set; }

    public virtual MaintenanceSchedule Schedule { get; private init; } = default!;

    private MaintenanceScheduleCheckpoint() { }

    public static MaintenanceScheduleCheckpoint Create(
        Guid scheduleId,
        int odometer,
        int months,
        int order,
        bool isCompleted = false,
        DateTimeOffset? completedAt = null,
        int? completedOdometer = null)
    {
        return new MaintenanceScheduleCheckpoint
        {
            ScheduleId = scheduleId,
            Odometer = odometer,
            Months = months,
            Order = order,
            IsCompleted = isCompleted,
            CompletedAt = completedAt,
            CompletedOdometer = completedOdometer
        };
    }

    public void Complete(int odometer, DateTimeOffset date)
    {
        IsCompleted = true;
        CompletedOdometer = odometer;
        CompletedAt = date;
        Touch();
    }

    public void MarkAsSuperceded(int currentOdometer, DateTimeOffset date)
    {
        IsCompleted = true;
        CompletedOdometer = currentOdometer;
        CompletedAt = date;
        Touch();
    }
}
