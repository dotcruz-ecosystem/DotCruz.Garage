using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.UseCases.Vehicles.Commands.RegisterVehicleSale;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace UseCases.Test.Vehicles;

public class RegisterVehicleSaleCommandHandlerTest
{
    private readonly IConfiguration _configuration;

    public RegisterVehicleSaleCommandHandlerTest()
    {
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Settings:FrontendUrl", "https://garage.dotcruz.com" }
            })
            .Build();
    }

    [Fact]
    public async Task Successfully_Registers_Sale_And_Schedules_PostSale_Maintenance()
    {
        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(workspaceId: workspaceId, userId: userId, initialOdometer: 40_000);

        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithSalesToUpdate(vehicle)
            .Build();

        var scheduledList = new List<MaintenanceSchedule>();
        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder()
            .SetupAdd(s => scheduledList.Add(s))
            .Build();

        var tokenService = new CheckinTokenServiceBuilder()
            .SetupGeneratePostSaleToken("valid.postsale.portal.token")
            .Build();

        var notificationService = new NotificationServiceBuilder().Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(workspaceId, userId)
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new RegisterVehicleSaleCommandHandler(
            vehicleWrite,
            scheduleWrite,
            tokenService,
            notificationService,
            workspaceProvider,
            _configuration,
            unitOfWork,
            NullLogger<RegisterVehicleSaleCommandHandler>.Instance
        );

        var command = RegisterVehicleSaleCommandBuilder.Build(
            vehicleId: vehicle.Id,
            customerName: "Carlos Oliveira",
            customerEmail: "carlos@gmail.com",
            soldOdometer: 42_000,
            soldPrice: 85_000m,
            warrantyKm: 5000,
            warrantyExpiresAt: DateTimeOffset.UtcNow.AddMonths(3)
        );

        var response = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotNull(response);
        Assert.Equal(vehicle.Id, response.VehicleId);
        Assert.Equal("Carlos Oliveira", response.CustomerName);
        Assert.Equal("carlos@gmail.com", response.CustomerEmail);
        Assert.Equal("valid.postsale.portal.token", response.PortalToken);
        Assert.Equal("https://garage.dotcruz.com/post-sale/portal?token=valid.postsale.portal.token", response.PortalUrl);

        Assert.True(vehicle.HasActivePostSale);
        Assert.NotNull(vehicle.ActiveSale);
        Assert.Equal("Carlos Oliveira", vehicle.ActiveSale.CustomerName);
        Assert.Equal(42_000, vehicle.CurrentOdometer);

        // Schedules created: Oil change + warranty inspection
        Assert.Equal(2, scheduledList.Count);
        Assert.Contains(scheduledList, s => s.Description.Contains("Troca de Óleo"));
        Assert.Contains(scheduledList, s => s.Description.Contains("Revisão de Garantia"));
    }

    [Fact]
    public async Task Throws_NotFoundException_When_Vehicle_Does_Not_Exist()
    {
        var (_, vehicleWrite) = new VehicleRepositoryBuilder()
            .SetupGetByIdWithSalesToUpdate(null)
            .Build();

        var (_, scheduleWrite) = new MaintenanceScheduleRepositoryBuilder().Build();
        var tokenService = new CheckinTokenServiceBuilder().Build();
        var notificationService = new NotificationServiceBuilder().Build();
        var workspaceProvider = new WorkspaceProviderBuilder().Build();
        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new RegisterVehicleSaleCommandHandler(
            vehicleWrite,
            scheduleWrite,
            tokenService,
            notificationService,
            workspaceProvider,
            _configuration,
            unitOfWork,
            NullLogger<RegisterVehicleSaleCommandHandler>.Instance
        );

        var command = RegisterVehicleSaleCommandBuilder.Build();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, TestContext.Current.CancellationToken));
    }
}
