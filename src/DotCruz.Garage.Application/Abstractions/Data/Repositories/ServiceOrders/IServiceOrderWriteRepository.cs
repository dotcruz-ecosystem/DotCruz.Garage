using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.ServiceOrders;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.ServiceOrders;

public interface IServiceOrderWriteRepository : IBaseWriteRepository<ServiceOrder>
{
    Task<ServiceOrder?> GetByIdWithDetailsToUpdateAsync(Guid id, CancellationToken cancellationToken);
}
