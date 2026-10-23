using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Enums;

namespace DotCruz.Garage.Domain.Entities.Vehicles;

public class VehicleModel : BaseEntity
{
    public Guid BrandId { get; private set; }
    public string Name { get; private set; } = null!;
    public VehicleType Type { get; private set; }

    public virtual VehicleBrand Brand { get; private set; } = null!;

    private VehicleModel() { }

    public static VehicleModel Create(Guid brandId, string name, VehicleType type)
    {
        return new VehicleModel
        {
            BrandId = brandId,
            Name = name.Trim(),
            Type = type
        };
    }
}
