namespace DotCruz.Garage.Domain.Entities.Base;

public interface ISoftDeleteEntity
{
    DateTimeOffset? DeletedAt { get; }
}
