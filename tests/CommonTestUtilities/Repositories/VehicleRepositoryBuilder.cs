using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Domain.Entities.Vehicles;
using Moq;

namespace CommonTestUtilities.Repositories;

public class VehicleRepositoryBuilder
{
    private readonly Mock<IVehicleReadRepository> _readMock = new();
    private readonly Mock<IVehicleWriteRepository> _writeMock = new();

    public Mock<IVehicleReadRepository> ReadMock => _readMock;
    public Mock<IVehicleWriteRepository> WriteMock => _writeMock;

    public VehicleRepositoryBuilder SetupGetById(Vehicle? vehicle)
    {
        _readMock.Setup(r => r.GetByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        return this;
    }

    public (IVehicleReadRepository Read, IVehicleWriteRepository Write) Build() =>
        (_readMock.Object, _writeMock.Object);
}
