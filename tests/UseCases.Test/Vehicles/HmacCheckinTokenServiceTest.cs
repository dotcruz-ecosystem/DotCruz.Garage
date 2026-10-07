using DotCruz.Garage.Infrastructure.Services.Security;
using Microsoft.Extensions.Configuration;

namespace UseCases.Test.Vehicles;

public class HmacCheckinTokenServiceTest
{
    private readonly HmacCheckinTokenService _service;

    public HmacCheckinTokenServiceTest()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Settings:OdometerTokenSecret", "VerySecretKeyForTestingOdometerTokenService2026" }
            })
            .Build();

        _service = new HmacCheckinTokenService(configuration);
    }

    [Fact]
    public void Generated_Token_Can_Be_Validated_Successfully()
    {
        var vehicleId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var token = _service.GenerateToken(vehicleId, workspaceId, TimeSpan.FromHours(48));

        Assert.NotNull(token);
        Assert.Contains(".", token);

        var isValid = _service.TryValidateToken(token, out var tokenVo);

        Assert.True(isValid);
        Assert.NotNull(tokenVo);
        Assert.Equal(vehicleId, tokenVo.VehicleId);
        Assert.Equal(workspaceId, tokenVo.WorkspaceId);
        Assert.False(tokenVo.IsExpired());
    }

    [Fact]
    public void Tampered_Token_Fails_Validation()
    {
        var vehicleId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var token = _service.GenerateToken(vehicleId, workspaceId, TimeSpan.FromHours(48));
        var parts = token.Split('.');
        var tamperedToken = $"{parts[0]}a.{parts[1]}";

        var isValid = _service.TryValidateToken(tamperedToken, out var tokenVo);

        Assert.False(isValid);
        Assert.Null(tokenVo);
    }

    [Fact]
    public void Expired_Token_Is_Detected()
    {
        var vehicleId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var token = _service.GenerateToken(vehicleId, workspaceId, TimeSpan.FromSeconds(-10));

        var isValid = _service.TryValidateToken(token, out var tokenVo);

        Assert.True(isValid);
        Assert.NotNull(tokenVo);
        Assert.True(tokenVo.IsExpired());
    }

    [Fact]
    public void Generated_Post_Sale_Token_Can_Be_Validated_Successfully()
    {
        var vehicleId = Guid.NewGuid();
        var saleId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var token = _service.GeneratePostSaleToken(vehicleId, saleId, workspaceId, TimeSpan.FromDays(180));

        Assert.NotNull(token);
        Assert.Contains(".", token);

        var isValid = _service.TryValidatePostSaleToken(token, out var tokenVo);

        Assert.True(isValid);
        Assert.NotNull(tokenVo);
        Assert.Equal(vehicleId, tokenVo.VehicleId);
        Assert.Equal(saleId, tokenVo.SaleId);
        Assert.Equal(workspaceId, tokenVo.WorkspaceId);
        Assert.False(tokenVo.IsExpired());
    }

    [Fact]
    public void Tampered_Post_Sale_Token_Fails_Validation()
    {
        var vehicleId = Guid.NewGuid();
        var saleId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var token = _service.GeneratePostSaleToken(vehicleId, saleId, workspaceId, TimeSpan.FromDays(180));
        var parts = token.Split('.');
        var tamperedToken = $"{parts[0]}xyz.{parts[1]}";

        var isValid = _service.TryValidatePostSaleToken(tamperedToken, out var tokenVo);

        Assert.False(isValid);
        Assert.Null(tokenVo);
    }
}
