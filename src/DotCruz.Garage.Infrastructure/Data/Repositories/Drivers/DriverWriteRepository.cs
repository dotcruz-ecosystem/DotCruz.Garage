using DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;
using DotCruz.Garage.Domain.Entities.Drivers;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Drivers;

public class DriverWriteRepository(GarageDbContext context)
    : BaseWriteRepository<Driver>(context), IDriverWriteRepository
{
    public async Task<Driver?> GetByIdToUpdateWithAssignmentsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(d => d.Assignments)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }
}
