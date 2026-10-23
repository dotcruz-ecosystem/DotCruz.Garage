using DotCruz.Garage.Domain.Entities.Base;

namespace DotCruz.Garage.Domain.Entities.Vehicles;

public class VehicleBrand : BaseEntity
{
    public string Name { get; private set; } = null!;

    public virtual ICollection<VehicleModel> Models { get; private init; } = new List<VehicleModel>();

    private VehicleBrand() { }

    public static VehicleBrand Create(string name)
    {
        return new VehicleBrand
        {
            Name = name.Trim()
        };
    }
}
