using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Maintenance;

public class MaintenanceLogWriteRepository(GarageDbContext context)
    : BaseWriteRepository<MaintenanceLog>(context), IMaintenanceLogWriteRepository;
