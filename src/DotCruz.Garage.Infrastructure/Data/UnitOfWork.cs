using DotCruz.Garage.Application.Abstractions.Data;

namespace DotCruz.Garage.Infrastructure.Data;

public class UnitOfWork(GarageDbContext context) : IUnitOfWork
{
    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
