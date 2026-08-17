using DotCruz.Garage.Domain.Entities.Base;

namespace DotCruz.Garage.Domain.Entities.Vehicles;

public class Vehicle : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Brand { get; private set; } = null!;
    public string Model { get; private set; } = null!;
    public int Year { get; private set; }
    public string? Plate { get; private set; }
    public string? Vin { get; private set; }
    public int CurrentOdometer { get; private set; }
    public double DailyMileageAverage { get; private set; }
    public DateTimeOffset? OdometerUpdatedAt { get; private set; }

    public virtual ICollection<OdometerHistory> OdometerHistories { get; private init; } = new List<OdometerHistory>();

    private Vehicle() { }

    public static Vehicle Create(
        Guid tenantId,
        Guid userId,
        string name,
        string brand,
        string model,
        int year,
        string? plate,
        string? vin,
        int initialOdometer,
        double initialDailyMileageAverage)
    {
        var vehicle = new Vehicle
        {
            TenantId = tenantId,
            UserId = userId,
            Name = name,
            Brand = brand,
            Model = model,
            Year = year,
            Plate = plate,
            Vin = vin,
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
}
