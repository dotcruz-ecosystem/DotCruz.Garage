using DotCruz.Garage.Domain.Entities.Base;

namespace DotCruz.Garage.Domain.Entities.Vehicles;

public class OdometerHistory : BaseEntity
{
    public Guid VehicleId { get; private set; }
    public int OdometerValue { get; private set; }
    public DateTimeOffset RecordedAt { get; private set; }

    private OdometerHistory() { }

    public static OdometerHistory Create(Guid vehicleId, int odometerValue, DateTimeOffset recordedAt)
    {
        return new OdometerHistory
        {
            VehicleId = vehicleId,
            OdometerValue = odometerValue,
            RecordedAt = recordedAt
        };
    }
}
