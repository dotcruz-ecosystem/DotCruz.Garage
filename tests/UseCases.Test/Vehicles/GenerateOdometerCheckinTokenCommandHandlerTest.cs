using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.GenerateOdometerCheckinToken;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace UseCases.Test.Vehicles;

public class GenerateOdometerCheckinTokenCommandHandlerTest
{
    [Fact]
    public async Task Successfully_Generates_Checkin_Token_And_Magic_Link()
    {
        var workspaceId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(workspaceId: workspaceId);

        var (vehicleRead, _) = new VehicleRepositoryBuilder()
            .SetupGetById(vehicle)
            .Build();

        var tokenService = new CheckinTokenServiceBuilder()
            .SetupGenerateToken("generated.hmac.token");

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(workspaceId, vehicle.UserId)
            .Build();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Settings:FrontendUrl", "https://garage.dotcruz.com" }
            })
            .Build();

        var handler = new GenerateOdometerCheckinTokenCommandHandler(
            vehicleRead,
            tokenService.Build(),
            workspaceProvider,
            config);

        var result = await handler.Handle(
            new GenerateOdometerCheckinTokenCommand(vehicle.Id, TimeSpan.FromHours(48)),
            TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(vehicle.Id, result.VehicleId);
        Assert.Equal("generated.hmac.token", result.Token);
        Assert.Equal("https://garage.dotcruz.com/checkin/odometer?token=generated.hmac.token", result.MagicLinkUrl);
    }

    [Fact]
    public async Task Throws_NotFoundException_When_Vehicle_Not_Found()
    {
        var (vehicleRead, _) = new VehicleRepositoryBuilder()
            .SetupGetById(null)
            .Build();

        var tokenService = new CheckinTokenServiceBuilder();
        var workspaceProvider = new WorkspaceProviderBuilder().Build();
        var config = new ConfigurationBuilder().Build();

        var handler = new GenerateOdometerCheckinTokenCommandHandler(
            vehicleRead,
            tokenService.Build(),
            workspaceProvider,
            config);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new GenerateOdometerCheckinTokenCommand(Guid.NewGuid()), TestContext.Current.CancellationToken));
    }
}
