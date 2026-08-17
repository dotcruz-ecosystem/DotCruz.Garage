namespace DotCruz.Garage.Application.Abstractions.Data;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken);
}
