using DotCruz.Garage.Application.Abstractions.Data.Repositories.Accessories;
using DotCruz.Garage.Domain.Entities.Accessories;
using Moq;

namespace CommonTestUtilities.Repositories;

public class VehicleAccessoryRepositoryBuilder
{
    private readonly Mock<IVehicleAccessoryReadRepository> _readMock = new();
    private readonly Mock<IVehicleAccessoryWriteRepository> _writeMock = new();

    public Mock<IVehicleAccessoryReadRepository> ReadMock => _readMock;
    public Mock<IVehicleAccessoryWriteRepository> WriteMock => _writeMock;

    public VehicleAccessoryRepositoryBuilder SetupGetAll(IReadOnlyCollection<VehicleAccessory> accessories)
    {
        _readMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessories);

        return this;
    }

    public VehicleAccessoryRepositoryBuilder SetupGetByVehicleId(IReadOnlyCollection<VehicleAccessory> accessories)
    {
        _readMock.Setup(r => r.GetByVehicleIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessories);

        return this;
    }

    public (IVehicleAccessoryReadRepository Read, IVehicleAccessoryWriteRepository Write) Build() =>
        (_readMock.Object, _writeMock.Object);
}
