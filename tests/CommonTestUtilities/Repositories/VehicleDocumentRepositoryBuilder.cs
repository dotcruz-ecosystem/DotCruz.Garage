using DotCruz.Garage.Application.Abstractions.Data.Repositories.Documents;
using DotCruz.Garage.Domain.Entities.Documents;
using Moq;

namespace CommonTestUtilities.Repositories;

public class VehicleDocumentRepositoryBuilder
{
    private readonly Mock<IVehicleDocumentReadRepository> _readMock = new();
    private readonly Mock<IVehicleDocumentWriteRepository> _writeMock = new();

    public Mock<IVehicleDocumentReadRepository> ReadMock => _readMock;
    public Mock<IVehicleDocumentWriteRepository> WriteMock => _writeMock;

    public VehicleDocumentRepositoryBuilder SetupGetAll(IReadOnlyCollection<VehicleDocument> documents)
    {
        _readMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(documents);

        return this;
    }

    public VehicleDocumentRepositoryBuilder SetupGetByVehicleId(IReadOnlyCollection<VehicleDocument> documents)
    {
        _readMock.Setup(r => r.GetByVehicleIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(documents);

        return this;
    }

    public (IVehicleDocumentReadRepository Read, IVehicleDocumentWriteRepository Write) Build() =>
        (_readMock.Object, _writeMock.Object);
}
