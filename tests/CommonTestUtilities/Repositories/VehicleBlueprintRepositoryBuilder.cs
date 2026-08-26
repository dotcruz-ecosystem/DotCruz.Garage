using DotCruz.Garage.Application.Abstractions.Data.Repositories.Catalog;
using DotCruz.Garage.Domain.Entities.Catalog;
using Moq;

namespace CommonTestUtilities.Repositories;

public class VehicleBlueprintRepositoryBuilder
{
    private readonly Mock<IVehicleBlueprintReadRepository> _readMock = new();
    private readonly Mock<IVehicleBlueprintWriteRepository> _writeMock = new();

    public Mock<IVehicleBlueprintReadRepository> ReadMock => _readMock;
    public Mock<IVehicleBlueprintWriteRepository> WriteMock => _writeMock;

    public VehicleBlueprintRepositoryBuilder SetupGetByDetails(VehicleBlueprint? blueprint)
    {
        _readMock.Setup(r => r.GetByDetailsWithTemplatesAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(blueprint);

        return this;
    }

    public VehicleBlueprintRepositoryBuilder SetupGetById(VehicleBlueprint? blueprint)
    {
        _readMock.Setup(r => r.GetByIdWithTemplatesAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(blueprint);

        return this;
    }

    public VehicleBlueprintRepositoryBuilder SetupSearch(IReadOnlyCollection<VehicleBlueprint> items, int totalCount)
    {
        _readMock.Setup(r => r.SearchAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((items, totalCount));

        return this;
    }

    public (IVehicleBlueprintReadRepository Read, IVehicleBlueprintWriteRepository Write) Build() =>
        (_readMock.Object, _writeMock.Object);
}
