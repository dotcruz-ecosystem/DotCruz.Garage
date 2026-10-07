using DotCruz.Garage.Application.Abstractions.Data.Repositories.Fines;
using DotCruz.Garage.Domain.Entities.Fines;
using Moq;

namespace CommonTestUtilities.Repositories;

public class TrafficFineRepositoryBuilder
{
    private readonly Mock<ITrafficFineReadRepository> _readMock = new();
    private readonly Mock<ITrafficFineWriteRepository> _writeMock = new();

    public Mock<ITrafficFineReadRepository> ReadMock => _readMock;
    public Mock<ITrafficFineWriteRepository> WriteMock => _writeMock;

    public TrafficFineRepositoryBuilder SetupGetAll(IReadOnlyCollection<TrafficFine> fines)
    {
        _readMock.Setup(r => r.GetAllAsync(
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(fines);

        return this;
    }

    public TrafficFineRepositoryBuilder SetupGetByVehicleId(IReadOnlyCollection<TrafficFine> fines)
    {
        _readMock.Setup(r => r.GetByVehicleIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(fines);

        return this;
    }

    public TrafficFineRepositoryBuilder SetupGetById(TrafficFine? fine)
    {
        _readMock.Setup(r => r.GetByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(fine);

        return this;
    }

    public TrafficFineRepositoryBuilder SetupGetByIdToUpdate(TrafficFine? fine)
    {
        _writeMock.Setup(w => w.GetByIdToUpdateAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(fine);

        return this;
    }

    public TrafficFineRepositoryBuilder SetupGetPendingNICFines(IReadOnlyCollection<TrafficFine> fines)
    {
        _readMock.Setup(r => r.GetPendingNICFinesAsync(
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(fines);

        return this;
    }

    public TrafficFineRepositoryBuilder SetupGetPendingFinesToNotify(IReadOnlyCollection<TrafficFine> fines)
    {
        _writeMock.Setup(w => w.GetPendingFinesToNotifyAsync(
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(fines);

        return this;
    }

    public (ITrafficFineReadRepository Read, ITrafficFineWriteRepository Write) Build() =>
        (_readMock.Object, _writeMock.Object);
}
