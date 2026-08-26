using DotCruz.Garage.Application.Abstractions.Data.Repositories.Catalog;
using DotCruz.Garage.Domain.Entities.Catalog;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Catalog;

public class VehicleBlueprintWriteRepository(GarageDbContext context)
    : BaseWriteRepository<VehicleBlueprint>(context), IVehicleBlueprintWriteRepository
{
}
