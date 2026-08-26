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

    public MaintenanceScheduleRepositoryBuilder SetupGetById(MaintenanceSchedule? schedule)
    {
        _readMock.Setup(r => r.GetByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        return this;
    }

    public MaintenanceScheduleRepositoryBuilder SetupGetByIdToUpdate(MaintenanceSchedule? schedule)
    {
        _writeMock.Setup(w => w.GetByIdToUpdateAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        return this;
    }

    public MaintenanceScheduleRepositoryBuilder SetupGetByVehicleId(IReadOnlyCollection<MaintenanceSchedule> schedules)
    {
        _readMock.Setup(r => r.GetByVehicleIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedules);

        return this;
    }

    public MaintenanceScheduleRepositoryBuilder SetupGetByVehicleIdToUpdate(IReadOnlyCollection<MaintenanceSchedule> schedules)
    {
        _writeMock.Setup(w => w.GetByVehicleIdToUpdateAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedules);

        return this;
    }

    public MaintenanceScheduleRepositoryBuilder SetupGetByVehicleIdToUpdateUnscoped(IReadOnlyCollection<MaintenanceSchedule> schedules)
    {
        _writeMock.Setup(w => w.GetByVehicleIdToUpdateUnscopedAsync(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedules);

        return this;
    }

    public (IMaintenanceScheduleReadRepository Read, IMaintenanceScheduleWriteRepository Write) Build() =>
        (_readMock.Object, _writeMock.Object);
}
