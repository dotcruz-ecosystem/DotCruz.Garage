using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Domain.Entities.Maintenance;
using Moq;

namespace CommonTestUtilities.Repositories;

public class MaintenanceScheduleRepositoryBuilder
{
    private readonly Mock<IMaintenanceScheduleReadRepository> _readMock = new();
    private readonly Mock<IMaintenanceScheduleWriteRepository> _writeMock = new();

    public Mock<IMaintenanceScheduleReadRepository> ReadMock => _readMock;
    public Mock<IMaintenanceScheduleWriteRepository> WriteMock => _writeMock;

    public (IMaintenanceScheduleReadRepository Read, IMaintenanceScheduleWriteRepository Write) Build() =>
        (_readMock.Object, _writeMock.Object);
}
