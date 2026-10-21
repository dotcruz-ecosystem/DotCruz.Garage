using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Domain.Entities.Maintenance;
using Moq;

namespace CommonTestUtilities.Repositories;

public class MaintenanceLogRepositoryBuilder
{
    private readonly Mock<IMaintenanceLogReadRepository> _readMock = new();
    private readonly Mock<IMaintenanceLogWriteRepository> _writeMock = new();

    public Mock<IMaintenanceLogReadRepository> ReadMock => _readMock;
    public Mock<IMaintenanceLogWriteRepository> WriteMock => _writeMock;

    public MaintenanceLogRepositoryBuilder SetupGetAll(IReadOnlyCollection<MaintenanceLog> logs)
    {
        _readMock.Setup(r => r.GetAllAsync(
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(logs);

        return this;
    }

    public MaintenanceLogRepositoryBuilder SetupGetByVehicleIdPaginated(IReadOnlyCollection<MaintenanceLog> items, int totalCount)
    {
        _readMock.Setup(r => r.GetByVehicleIdPaginatedAsync(
            It.IsAny<Guid>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((items, totalCount));

        return this;
    }

    public MaintenanceLogRepositoryBuilder SetupGetByVehicleIdUnscoped(IReadOnlyCollection<MaintenanceLog> logs)
    {
        _readMock.Setup(r => r.GetByVehicleIdUnscopedAsync(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(logs);

        return this;
    }

    public MaintenanceLogRepositoryBuilder SetupGetByIdWithAttachmentsToUpdate(MaintenanceLog? log)
    {
        _writeMock.Setup(w => w.GetByIdWithAttachmentsToUpdateAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(log);

        return this;
    }

    public (IMaintenanceLogReadRepository Read, IMaintenanceLogWriteRepository Write) Build() =>
        (_readMock.Object, _writeMock.Object);
}
