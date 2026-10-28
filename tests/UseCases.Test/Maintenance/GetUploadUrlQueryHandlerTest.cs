using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.UseCases.Maintenance.Queries.GetUploadUrl;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;

namespace UseCases.Test.Maintenance;

public class GetUploadUrlQueryHandlerTest
{
    [Fact]
    public async Task Successfully_Generates_Presigned_Upload_Url_For_Vehicle()
    {
        var vehicle = VehicleBuilder.Build();
        var (vehicleRead, _) = new VehicleRepositoryBuilder()
            .SetupGetById(vehicle)
            .Build();

        var storageService = new StorageServiceBuilder()
            .SetupGeneratePresignedUploadUrl("https://r2.upload.url", "https://pub.r2.dev/file.pdf")
            .Build();

        var workspaceProvider = new WorkspaceProviderBuilder()
            .AsUser(vehicle.Workspaces.First().WorkspaceId, vehicle.UserId)
            .Build();

        var handler = new GetUploadUrlQueryHandler(vehicleRead, storageService, workspaceProvider);

        var query = new GetUploadUrlQuery(vehicle.Id, "nota.pdf", "application/pdf", 500_000);
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal("https://r2.upload.url", result.UploadUrl);
        Assert.Equal("https://pub.r2.dev/file.pdf", result.PublicUrl);
    }

    [Fact]
    public async Task Throws_NotFoundException_When_Vehicle_Does_Not_Exist()
    {
        var (vehicleRead, _) = new VehicleRepositoryBuilder()
            .SetupGetById(null)
            .Build();

        var storageService = new StorageServiceBuilder().Build();
        var workspaceProvider = new WorkspaceProviderBuilder().Build();

        var handler = new GetUploadUrlQueryHandler(vehicleRead, storageService, workspaceProvider);

        var query = new GetUploadUrlQuery(Guid.NewGuid(), "nota.pdf", "application/pdf", 500_000);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(query, TestContext.Current.CancellationToken));
    }
}
