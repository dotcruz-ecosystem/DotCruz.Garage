using DotCruz.Garage.Application.Abstractions.Data.Repositories.Accessories;
using DotCruz.Garage.Domain.Entities.Accessories;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Accessories;

public class VehicleAccessoryWriteRepository(GarageDbContext context)
    : BaseWriteRepository<VehicleAccessory>(context), IVehicleAccessoryWriteRepository;
