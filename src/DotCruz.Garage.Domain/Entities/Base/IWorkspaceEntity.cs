namespace DotCruz.Garage.Domain.Entities.Base;

public interface IWorkspaceEntity
{
    Guid WorkspaceId { get; }
    Guid UserId { get; }
}
