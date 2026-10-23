using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;
using Moq;

namespace CommonTestUtilities.Repositories;

public class VehicleCatalogRepositoryBuilder
{
    private readonly Mock<IVehicleCatalogRepository> _mock = new();

    public Mock<IVehicleCatalogRepository> Mock => _mock;

    public VehicleCatalogRepositoryBuilder SetupGetBrands(IReadOnlyCollection<string> brands)
    {
        _mock.Setup(r => r.GetBrandsAsync(
            It.IsAny<VehicleType?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(brands);

        return this;
    }

    public VehicleCatalogRepositoryBuilder SetupGetModels(IReadOnlyCollection<string> models)
    {
        _mock.Setup(r => r.GetModelsAsync(
            It.IsAny<string>(),
            It.IsAny<VehicleType?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(models);

        return this;
    }

    public VehicleCatalogRepositoryBuilder SetupGetEngines(IReadOnlyCollection<string> engines)
    {
        _mock.Setup(r => r.GetEnginesAsync(
            It.IsAny<VehicleType?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(engines);

        return this;
    }

    public VehicleCatalogRepositoryBuilder SetupEnsureBrandExists(VehicleBrand? brand = null)
    {
        _mock.Setup(r => r.EnsureBrandExistsAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((string name, CancellationToken _) => brand ?? VehicleBrand.Create(name));

        return this;
    }

    public VehicleCatalogRepositoryBuilder SetupEnsureModelExists(VehicleModel? model = null)
    {
        _mock.Setup(r => r.EnsureModelExistsAsync(
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<VehicleType>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid brandId, string name, VehicleType type, CancellationToken _) => model ?? VehicleModel.Create(brandId, name, type));

        return this;
    }

    public VehicleCatalogRepositoryBuilder SetupEnsureEngineExists(VehicleEngine? engine = null)
    {
        _mock.Setup(r => r.EnsureEngineExistsAsync(
            It.IsAny<string>(),
            It.IsAny<VehicleType?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((string name, VehicleType? type, CancellationToken _) => engine ?? VehicleEngine.Create(name, type));

        return this;
    }

    public IVehicleCatalogRepository Build()
    {
        // Setup default behaviors if not explicitly configured
        _mock.Setup(r => r.EnsureBrandExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string name, CancellationToken _) => VehicleBrand.Create(name));

        _mock.Setup(r => r.EnsureModelExistsAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<VehicleType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid brandId, string name, VehicleType type, CancellationToken _) => VehicleModel.Create(brandId, name, type));

        _mock.Setup(r => r.EnsureEngineExistsAsync(It.IsAny<string>(), It.IsAny<VehicleType?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string name, VehicleType? type, CancellationToken _) => VehicleEngine.Create(name, type));

        return _mock.Object;
    }
}
