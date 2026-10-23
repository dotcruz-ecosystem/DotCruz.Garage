using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;
using Moq;

namespace CommonTestUtilities.Repositories;

public class VehicleBlueprintRepositoryBuilder
{
    private readonly Mock<IVehicleBlueprintRepository> _mock = new();
    private readonly List<VehicleBlueprint> _blueprints = new();

    private bool _hasCustomMatching;
    private bool _hasCustomGetById;
    private bool _hasCustomPending;
    private bool _hasCustomAdd;

    public Mock<IVehicleBlueprintRepository> Mock => _mock;

    public VehicleBlueprintRepositoryBuilder WithBlueprint(VehicleBlueprint blueprint)
    {
        _blueprints.Add(blueprint);
        return this;
    }

    public VehicleBlueprintRepositoryBuilder SetupFindMatchingBlueprint(VehicleBlueprint? blueprint)
    {
        _hasCustomMatching = true;
        _mock.Setup(r => r.FindMatchingBlueprintAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<VehicleType>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(blueprint);

        return this;
    }

    public VehicleBlueprintRepositoryBuilder SetupGetByIdWithTemplates(VehicleBlueprint? blueprint)
    {
        _hasCustomGetById = true;
        _mock.Setup(r => r.GetByIdWithTemplatesAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(blueprint);

        return this;
    }

    public VehicleBlueprintRepositoryBuilder SetupGetPendingBlueprints(IReadOnlyCollection<VehicleBlueprint> blueprints)
    {
        _hasCustomPending = true;
        _mock.Setup(r => r.GetPendingBlueprintsAsync(
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(blueprints);

        return this;
    }

    public VehicleBlueprintRepositoryBuilder SetupAdd(Action<VehicleBlueprint>? callback = null)
    {
        _hasCustomAdd = true;
        _mock.Setup(r => r.AddAsync(It.IsAny<VehicleBlueprint>(), It.IsAny<CancellationToken>()))
            .Callback<VehicleBlueprint, CancellationToken>((b, _) =>
            {
                _blueprints.Add(b);
                callback?.Invoke(b);
            })
            .Returns(Task.CompletedTask);

        return this;
    }

    public IVehicleBlueprintRepository Build()
    {
        if (!_hasCustomAdd)
        {
            _mock.Setup(r => r.AddAsync(It.IsAny<VehicleBlueprint>(), It.IsAny<CancellationToken>()))
                .Callback<VehicleBlueprint, CancellationToken>((b, _) => _blueprints.Add(b))
                .Returns(Task.CompletedTask);
        }

        if (!_hasCustomMatching)
        {
            _mock.Setup(r => r.FindMatchingBlueprintAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<VehicleType>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((string brand, string model, string engine, int year, VehicleType type, CancellationToken _) =>
                {
                    return _blueprints.FirstOrDefault(b =>
                        b.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase) &&
                        b.Model.Equals(model, StringComparison.OrdinalIgnoreCase) &&
                        b.Engine.Equals(engine, StringComparison.OrdinalIgnoreCase) &&
                        b.Type == type &&
                        b.YearFrom <= year && b.YearTo >= year);
                });
        }

        if (!_hasCustomGetById)
        {
            _mock.Setup(r => r.GetByIdWithTemplatesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken _) => _blueprints.FirstOrDefault(b => b.Id == id));
        }

        if (!_hasCustomPending)
        {
            _mock.Setup(r => r.GetPendingBlueprintsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((int limit, CancellationToken _) => _blueprints.Where(b => b.Status == BlueprintStatus.Pending).Take(limit).ToList());
        }

        return _mock.Object;
    }
}
