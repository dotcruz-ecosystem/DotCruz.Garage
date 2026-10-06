using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Domain.Entities.Vehicles;
using Moq;

namespace CommonTestUtilities.Repositories;

public class VehicleSaleRepositoryBuilder
{
    private readonly Mock<IVehicleSaleReadRepository> _readMock = new();
    private readonly Mock<IVehicleSaleWriteRepository> _writeMock = new();

    public Mock<IVehicleSaleReadRepository> ReadMock => _readMock;
    public Mock<IVehicleSaleWriteRepository> WriteMock => _writeMock;

    public VehicleSaleRepositoryBuilder SetupGetActiveSalesWithVehicle(IReadOnlyCollection<VehicleSale> sales)
    {
        _readMock.Setup(r => r.GetActiveSalesWithVehicleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sales);
        return this;
    }

    public VehicleSaleRepositoryBuilder SetupGetByIdWithVehicle(VehicleSale? sale)
    {
        _readMock.Setup(r => r.GetByIdWithVehicleAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        return this;
    }

    public VehicleSaleRepositoryBuilder SetupGetByVehicleIdWithVehicle(VehicleSale? sale)
    {
        _readMock.Setup(r => r.GetByVehicleIdWithVehicleAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        return this;
    }

    public VehicleSaleRepositoryBuilder SetupGetSaleUnscoped(VehicleSale? sale)
    {
        _readMock.Setup(r => r.GetSaleUnscopedAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        return this;
    }

    public VehicleSaleRepositoryBuilder SetupGetActiveSaleByVehicleId(VehicleSale? sale)
    {
        _writeMock.Setup(w => w.GetActiveSaleByVehicleIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        return this;
    }

    public VehicleSaleRepositoryBuilder SetupGetSaleToUpdateUnscoped(VehicleSale? sale)
    {
        _writeMock.Setup(w => w.GetSaleToUpdateUnscopedAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        return this;
    }

    public VehicleSaleRepositoryBuilder SetupAdd(Action<VehicleSale>? callback = null)
    {
        var setup = _writeMock.Setup(w => w.AddAsync(It.IsAny<VehicleSale>(), It.IsAny<CancellationToken>()));
        if (callback != null)
        {
            setup.Callback<VehicleSale, CancellationToken>((s, _) => callback(s));
        }
        return this;
    }

    public (IVehicleSaleReadRepository Read, IVehicleSaleWriteRepository Write) Build() =>
        (_readMock.Object, _writeMock.Object);
}
