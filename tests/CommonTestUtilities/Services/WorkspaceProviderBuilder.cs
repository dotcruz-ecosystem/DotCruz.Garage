using DotCruz.Shared.Security.Context;
using Moq;

namespace CommonTestUtilities.Services;

public class WorkspaceProviderBuilder
{
    private readonly Mock<IWorkspaceProvider> _provider = new();

    public WorkspaceProviderBuilder AsService()
    {
        _provider.Setup(p => p.WorkspaceId).Returns((Guid?)null);
        _provider.Setup(p => p.UserId).Returns((Guid?)null);
        _provider.Setup(p => p.FilterByUserId).Returns(true);

        return this;
    }

    public WorkspaceProviderBuilder AsWorkspace(Guid workspaceId, Guid? userId = null, bool filterByUserId = false)
    {
        _provider.Setup(p => p.WorkspaceId).Returns(workspaceId);
        _provider.Setup(p => p.UserId).Returns(userId ?? Guid.NewGuid());
        _provider.Setup(p => p.FilterByUserId).Returns(filterByUserId);

        return this;
    }

    public WorkspaceProviderBuilder AsUser(Guid workspaceId, Guid userId, bool filterByUserId = true)
    {
        _provider.Setup(p => p.WorkspaceId).Returns(workspaceId);
        _provider.Setup(p => p.UserId).Returns(userId);
        _provider.Setup(p => p.FilterByUserId).Returns(filterByUserId);

        return this;
    }

    public IWorkspaceProvider Build() => _provider.Object;
}
