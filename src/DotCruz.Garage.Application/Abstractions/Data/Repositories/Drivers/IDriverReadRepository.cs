using DotCruz.Garage.Application.Abstractions.Data.Repositories.Base;
using DotCruz.Garage.Domain.Entities.Drivers;

namespace DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;

public interface IDriverReadRepository : IBaseReadRepository<Driver>
{
    Task<Driver?> GetByIdWithFinesAsync(Guid id, CancellationToken cancellationToken);
    Task<Driver?> GetByCpfAsync(string cpf, CancellationToken cancellationToken);
    Task<(IReadOnlyCollection<Driver> Items, int TotalCount)> SearchAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm,
        bool? activeOnly,
        CancellationToken cancellationToken);
}
