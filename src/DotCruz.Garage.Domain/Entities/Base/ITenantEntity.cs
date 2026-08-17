namespace DotCruz.Garage.Domain.Entities.Base;

public interface ITenantEntity
{
    Guid TenantId { get; }
    Guid UserId { get; }
}
