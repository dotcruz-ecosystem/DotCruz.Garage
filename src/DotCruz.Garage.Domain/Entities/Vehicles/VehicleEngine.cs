using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Domain.Entities.Vehicles;

public class VehicleEngine : BaseEntity
{
    public string Name { get; private set; } = null!;
    public VehicleType? Type { get; private set; }

    private VehicleEngine() { }

    public static VehicleEngine Create(string name, VehicleType? type = null)
    {
        return new VehicleEngine
        {
            Name = name.Trim(),
            Type = type
        };
    }
}
