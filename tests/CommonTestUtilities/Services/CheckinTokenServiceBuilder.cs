using DotCruz.Garage.Application.Abstractions.Services.Security;
using DotCruz.Garage.Domain.ValueObjects;
using Moq;

namespace CommonTestUtilities.Services;

public class CheckinTokenServiceBuilder
{
    private readonly Mock<ICheckinTokenService> _mock = new();

    public Mock<ICheckinTokenService> Mock => _mock;

    public CheckinTokenServiceBuilder SetupGenerateToken(string token = "valid.token")
    {
        _mock.Setup(s => s.GenerateToken(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<TimeSpan?>()))
            .Returns(token);
        return this;
    }

    public CheckinTokenServiceBuilder SetupValidToken(string tokenString, OdometerCheckinToken token)
    {
        OdometerCheckinToken? outToken = token;
        _mock.Setup(s => s.TryValidateToken(tokenString, out outToken))
            .Returns(true);
        return this;
    }

    public CheckinTokenServiceBuilder SetupInvalidToken(string tokenString)
    {
        OdometerCheckinToken? nullToken = null;
        _mock.Setup(s => s.TryValidateToken(tokenString, out nullToken))
            .Returns(false);
        return this;
    }

    public CheckinTokenServiceBuilder SetupGeneratePostSaleToken(string token = "valid.postsale.token")
    {
        _mock.Setup(s => s.GeneratePostSaleToken(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<TimeSpan?>()))
            .Returns(token);
        return this;
    }

    public CheckinTokenServiceBuilder SetupValidPostSaleToken(string tokenString, PostSalePortalToken token)
    {
        PostSalePortalToken? outToken = token;
        _mock.Setup(s => s.TryValidatePostSaleToken(tokenString, out outToken))
            .Returns(true);
        return this;
    }

    public CheckinTokenServiceBuilder SetupInvalidPostSaleToken(string tokenString)
    {
        PostSalePortalToken? nullToken = null;
        _mock.Setup(s => s.TryValidatePostSaleToken(tokenString, out nullToken))
            .Returns(false);
        return this;
    }

    public ICheckinTokenService Build() => _mock.Object;
}
