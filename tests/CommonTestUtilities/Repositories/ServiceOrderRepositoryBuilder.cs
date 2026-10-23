using DotCruz.Garage.Application.Abstractions.Data.Repositories.ServiceOrders;
using DotCruz.Garage.Domain.Entities.ServiceOrders;
using Moq;

namespace CommonTestUtilities.Repositories;

public class ServiceOrderRepositoryBuilder
{
    private readonly Mock<IServiceOrderReadRepository> _readMock = new();
    private readonly Mock<IServiceOrderWriteRepository> _writeMock = new();

    public Mock<IServiceOrderReadRepository> ReadMock => _readMock;
    public Mock<IServiceOrderWriteRepository> WriteMock => _writeMock;

    public ServiceOrderRepositoryBuilder SetupGetByIdWithDetails(ServiceOrder? serviceOrder)
    {
        _readMock.Setup(r => r.GetByIdWithDetailsAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceOrder);

        return this;
    }

    public ServiceOrderRepositoryBuilder SetupGetByVehicleId(IReadOnlyCollection<ServiceOrder> orders)
    {
        _readMock.Setup(r => r.GetByVehicleIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        return this;
    }

    public ServiceOrderRepositoryBuilder SetupGetByVehicleIdPaginated(IReadOnlyCollection<ServiceOrder> items, int totalCount)
    {
        _readMock.Setup(r => r.GetByVehicleIdPaginatedAsync(
            It.IsAny<Guid>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((items, totalCount));

        return this;
    }

    public ServiceOrderRepositoryBuilder SetupGetByIdWithDetailsToUpdate(ServiceOrder? serviceOrder)
    {
        _writeMock.Setup(w => w.GetByIdWithDetailsToUpdateAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceOrder);

        return this;
    }

    public (IServiceOrderReadRepository Read, IServiceOrderWriteRepository Write) Build() =>
        (_readMock.Object, _writeMock.Object);
}
