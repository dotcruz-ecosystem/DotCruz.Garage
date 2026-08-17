namespace DotCruz.Garage.Domain.Entities.Base;

public abstract class BaseEntity : ISoftDeleteEntity
{
    public Guid Id { get; private init; } = Guid.CreateVersion7();
    public DateTimeOffset CreatedAt { get; private init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DeletedAt { get; private set; }

    public void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
    public void Delete() => DeletedAt = DateTimeOffset.UtcNow;
}
