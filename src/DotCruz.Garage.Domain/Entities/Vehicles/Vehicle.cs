using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Domain.Entities.Vehicles;

public class Vehicle : BaseEntity, IWorkspaceEntity
{
    public Guid WorkspaceId { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public VehicleType Type { get; private set; }
    public string Brand { get; private set; } = null!;
    public string Model { get; private set; } = null!;
    public string Engine { get; private set; } = null!;
    public int Year { get; private set; }
    public string? Plate { get; private set; }
    public int CurrentOdometer { get; private set; }
    public double DailyMileageAverage { get; private set; }
    public DateTimeOffset? OdometerUpdatedAt { get; private set; }

    public virtual ICollection<OdometerHistory> OdometerHistories { get; private init; } = new List<OdometerHistory>();

    private Vehicle() { }

    public static Vehicle Create(
        Guid workspaceId,
        Guid userId,
        string name,
        VehicleType type,
        string brand,
        string model,
        string engine,
        int year,
        string? plate,
        int initialOdometer,
        double initialDailyMileageAverage)
    {
        var vehicle = new Vehicle
        {
            WorkspaceId = workspaceId,
            UserId = userId,
            Name = name,
            Type = type,
            Brand = brand,
            Model = model,
            Engine = engine,
            Year = year,
            Plate = plate,
            CurrentOdometer = initialOdometer,
            DailyMileageAverage = initialDailyMileageAverage,
            OdometerUpdatedAt = DateTimeOffset.UtcNow
        };

        vehicle.OdometerHistories.Add(OdometerHistory.Create(vehicle.Id, initialOdometer, DateTimeOffset.UtcNow));

        return vehicle;
    }

    public void UpdateOdometer(int newOdometer, DateTimeOffset recordedAt)
    {
        if (newOdometer < CurrentOdometer)
        {
            throw new InvalidOperationException("A nova quilometragem não pode ser menor que a quilometragem atual.");
        }

        CurrentOdometer = newOdometer;
        OdometerUpdatedAt = recordedAt;
    }

    public void UpdateDailyMileageAverage(double newAverage)
    {
        if (newAverage < 0)
        {
            throw new ArgumentException("A média diária de quilometragem não pode ser negativa.", nameof(newAverage));
        }

        DailyMileageAverage = newAverage;
    }

    public bool RecordOdometerReading(int odometer, DateTimeOffset recordedAt)
    {
        bool alreadyExists = OdometerHistories.Any(h =>
            h.OdometerValue == odometer &&
            h.RecordedAt.UtcDateTime.Date == recordedAt.UtcDateTime.Date);

        if (!alreadyExists)
        {
            OdometerHistories.Add(OdometerHistory.Create(Id, odometer, recordedAt));
        }

        if (odometer > CurrentOdometer)
        {
            CurrentOdometer = odometer;
            OdometerUpdatedAt = recordedAt;
        }

        var newAverage = DotCruz.Garage.Domain.Services.OdometerCalculator.CalculateDailyAverage(
            OdometerHistories,
            DailyMileageAverage);

        UpdateDailyMileageAverage(newAverage);

        return !alreadyExists;
    }
}
