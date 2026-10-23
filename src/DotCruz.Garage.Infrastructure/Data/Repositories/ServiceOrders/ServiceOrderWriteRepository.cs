using DotCruz.Garage.Application.Abstractions.Data.Repositories.ServiceOrders;
using DotCruz.Garage.Domain.Entities.ServiceOrders;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.ServiceOrders;

public class ServiceOrderWriteRepository : BaseWriteRepository<ServiceOrder>, IServiceOrderWriteRepository
{
    public ServiceOrderWriteRepository(GarageDbContext context) : base(context)
    {
    }

    public async Task<ServiceOrder?> GetByIdWithDetailsToUpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(so => so.Items)
            .Include(so => so.Attachments)
            .FirstOrDefaultAsync(so => so.Id == id, cancellationToken);
    }
}
