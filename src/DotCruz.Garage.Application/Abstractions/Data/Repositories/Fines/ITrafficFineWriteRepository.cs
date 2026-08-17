using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Fines;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Fines;

public interface ITrafficFineWriteRepository : IBaseWriteRepository<TrafficFine>
{
    Task<IReadOnlyCollection<TrafficFine>> GetPendingFinesToNotifyAsync(CancellationToken cancellationToken);
}
