using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.UseCases.ServiceOrders.Queries.ExtractServiceOrderFromAttachment;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;

namespace UseCases.Test.ServiceOrders;

public class ExtractServiceOrderFromAttachmentQueryHandlerTest
{
    [Fact]
    public async Task Success_ExtractsServiceOrder_FromAttachment()
    {
        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(workspaceId: workspaceId, userId: userId, initialOdometer: 20_000);
        var schedule = MaintenanceScheduleBuilder.Build(
            workspaceId: workspaceId,
            userId: userId,
            vehicleId: vehicle.Id,
            currentOdometer: 20_000,
            intervalOdometer: 10_000);

        var (vehicleRead, _) = new VehicleRepositoryBuilder()
            .SetupGetById(vehicle)
            .Build();

        var (scheduleRead, _) = new MaintenanceScheduleRepositoryBuilder()
            .SetupGetByVehicleId(new[] { schedule })
            .Build();

        var aiService = new AiServiceOrderExtractionServiceBuilder().Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(workspaceId, userId)
            .Build();

        var handler = new ExtractServiceOrderFromAttachmentQueryHandler(
            vehicleRead,
            scheduleRead,
            aiService,
            workspaceProvider);

        var requestDto = ExtractServiceOrderFromAttachmentRequestBuilder.Build(
            fileUrl: "https://r2.dotcruz.com/os-moto.jpg",
            fileName: "os-moto.jpg",
            contentType: "image/jpeg");

        var query = new ExtractServiceOrderFromAttachmentQuery(vehicle.Id, requestDto);
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal("39196", result.OrderNumber);
        Assert.Equal("Moto Selvagem", result.WorkshopName);
        Assert.Equal(24891, result.OdometerAtService);
        Assert.Equal(178.70m, result.TotalCost);
        Assert.Equal("TAL-1G31", result.DetectedVehiclePlate);
        Assert.Equal("MT 03", result.DetectedVehicleModel);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("PARAFUSO PEDAL CAMBIO", result.Items[0].Description);
        Assert.Equal("136592MGL", result.Items[0].PartNumber);
        Assert.Equal(schedule.Id, result.Items[1].MatchedMaintenanceScheduleId);
    }

    [Fact]
    public async Task Throws_NotFound_When_Vehicle_Does_Not_Exist()
    {
        var (vehicleRead, _) = new VehicleRepositoryBuilder()
            .SetupGetById(null)
            .Build();

        var (scheduleRead, _) = new MaintenanceScheduleRepositoryBuilder().Build();
        var aiService = new AiServiceOrderExtractionServiceBuilder().Build();
        var workspaceProvider = new WorkspaceProviderBuilder().Build();

        var handler = new ExtractServiceOrderFromAttachmentQueryHandler(
            vehicleRead,
            scheduleRead,
            aiService,
            workspaceProvider);

        var query = new ExtractServiceOrderFromAttachmentQuery(
            Guid.NewGuid(),
            ExtractServiceOrderFromAttachmentRequestBuilder.Build());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(query, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Throws_Forbidden_When_Workspace_Cannot_Write()
    {
        var ownerWorkspaceId = Guid.NewGuid();
        var foreignWorkspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var vehicle = VehicleBuilder.Build(workspaceId: ownerWorkspaceId, userId: userId);

        var (vehicleRead, _) = new VehicleRepositoryBuilder()
            .SetupGetById(vehicle)
            .Build();

        var (scheduleRead, _) = new MaintenanceScheduleRepositoryBuilder().Build();
        var aiService = new AiServiceOrderExtractionServiceBuilder().Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(foreignWorkspaceId, userId)
            .Build();

        var handler = new ExtractServiceOrderFromAttachmentQueryHandler(
            vehicleRead,
            scheduleRead,
            aiService,
            workspaceProvider);

        var query = new ExtractServiceOrderFromAttachmentQuery(
            vehicle.Id,
            ExtractServiceOrderFromAttachmentRequestBuilder.Build());

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(query, TestContext.Current.CancellationToken));
    }
}
