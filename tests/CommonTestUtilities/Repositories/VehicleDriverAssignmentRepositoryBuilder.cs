using DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;
using DotCruz.Garage.Domain.Entities.Drivers;
using Moq;

namespace CommonTestUtilities.Repositories;

public class VehicleDriverAssignmentRepositoryBuilder
{
    private readonly Mock<IVehicleDriverAssignmentReadRepository> _readMock = new();
    private readonly Mock<IVehicleDriverAssignmentWriteRepository> _writeMock = new();

    public Mock<IVehicleDriverAssignmentReadRepository> ReadMock => _readMock;
    public Mock<IVehicleDriverAssignmentWriteRepository> WriteMock => _writeMock;

    public VehicleDriverAssignmentRepositoryBuilder SetupGetByVehicleId(IReadOnlyCollection<VehicleDriverAssignment> assignments)
    {
        _readMock.Setup(r => r.GetByVehicleIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignments);

        return this;
    }

    public VehicleDriverAssignmentRepositoryBuilder SetupGetCurrentByVehicleId(VehicleDriverAssignment? assignment)
    {
        _readMock.Setup(r => r.GetCurrentByVehicleIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignment);

        return this;
    }

    public VehicleDriverAssignmentRepositoryBuilder SetupGetCurrentByVehicleIdToUpdate(VehicleDriverAssignment? assignment)
    {
        _writeMock.Setup(w => w.GetCurrentByVehicleIdToUpdateAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignment);

        return this;
    }

    public (IVehicleDriverAssignmentReadRepository Read, IVehicleDriverAssignmentWriteRepository Write) Build() =>
        (_readMock.Object, _writeMock.Object);
}
