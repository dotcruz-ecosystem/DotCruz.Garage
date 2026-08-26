using DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;
using DotCruz.Garage.Domain.Entities.Drivers;
using DotCruz.Garage.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data.Repositories.Drivers;

public class DriverReadRepository(GarageDbContext context)
    : BaseReadRepository<Driver>(context), IDriverReadRepository
{
    public async Task<Driver?> GetByIdWithFinesAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.AsNoTracking()
            .Include(d => d.Fines)
            .Include(d => d.Assignments)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Driver?> GetByCpfAsync(string cpf, CancellationToken cancellationToken)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Cpf == cpf, cancellationToken);
    }

    public async Task<(IReadOnlyCollection<Driver> Items, int TotalCount)> SearchAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm,
        bool? activeOnly,
        CancellationToken cancellationToken)
    {
        var query = _dbSet.AsNoTracking().Include(d => d.Fines).AsQueryable();

        if (activeOnly.HasValue)
        {
            query = query.Where(d => d.IsActive == activeOnly.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(d =>
                d.Name.ToLower().Contains(term) ||
                d.Cpf.Contains(term) ||
                d.Cnh.Contains(term) ||
                d.Email.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(d => d.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
