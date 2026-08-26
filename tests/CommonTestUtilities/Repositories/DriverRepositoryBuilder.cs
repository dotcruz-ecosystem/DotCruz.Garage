using DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;
using DotCruz.Garage.Domain.Entities.Drivers;
using Moq;

namespace CommonTestUtilities.Repositories;

public class DriverRepositoryBuilder
{
    private readonly Mock<IDriverReadRepository> _readMock = new();
    private readonly Mock<IDriverWriteRepository> _writeMock = new();

    public Mock<IDriverReadRepository> ReadMock => _readMock;
    public Mock<IDriverWriteRepository> WriteMock => _writeMock;

    public DriverRepositoryBuilder SetupGetById(Driver? driver)
    {
        _readMock.Setup(r => r.GetByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);

        return this;
    }

    public DriverRepositoryBuilder SetupGetByIdWithFines(Driver? driver)
    {
        _readMock.Setup(r => r.GetByIdWithFinesAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);

        return this;
    }

    public DriverRepositoryBuilder SetupGetByCpf(Driver? driver)
    {
        _readMock.Setup(r => r.GetByCpfAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);

        return this;
    }

    public DriverRepositoryBuilder SetupGetByIdToUpdate(Driver? driver)
    {
        _writeMock.Setup(w => w.GetByIdToUpdateAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);

        return this;
    }

    public DriverRepositoryBuilder SetupSearch(IReadOnlyCollection<Driver> items, int totalCount)
    {
        _readMock.Setup(r => r.SearchAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string?>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((items, totalCount));

        return this;
    }

    public (IDriverReadRepository Read, IDriverWriteRepository Write) Build() =>
        (_readMock.Object, _writeMock.Object);
}
