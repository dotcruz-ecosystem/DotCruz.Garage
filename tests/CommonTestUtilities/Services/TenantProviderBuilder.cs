using DotCruz.Shared.Security.Context;
using Moq;

namespace CommonTestUtilities.Services;

public class TenantProviderBuilder
{
    private readonly Mock<ITenantProvider> _provider = new();

    public TenantProviderBuilder AsService()
    {
        _provider.Setup(p => p.TenantId).Returns((Guid?)null);
        _provider.Setup(p => p.UserId).Returns((Guid?)null);
        _provider.Setup(p => p.FilterByUserId).Returns(true);

        return this;
    }

    public TenantProviderBuilder AsTenant(Guid tenantId, Guid? userId = null, bool filterByUserId = false)
    {
        _provider.Setup(p => p.TenantId).Returns(tenantId);
        _provider.Setup(p => p.UserId).Returns(userId ?? Guid.NewGuid());
        _provider.Setup(p => p.FilterByUserId).Returns(filterByUserId);

        return this;
    }

    public TenantProviderBuilder AsUser(Guid tenantId, Guid userId, bool filterByUserId = true)
    {
        _provider.Setup(p => p.TenantId).Returns(tenantId);
        _provider.Setup(p => p.UserId).Returns(userId);
        _provider.Setup(p => p.FilterByUserId).Returns(filterByUserId);

        return this;
    }

    public ITenantProvider Build() => _provider.Object;
}
