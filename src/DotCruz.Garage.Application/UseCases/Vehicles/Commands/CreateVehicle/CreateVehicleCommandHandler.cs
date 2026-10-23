using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Shared.Security.Context;
using DotCruz.Garage.Application.Mappers.Vehicles;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.CreateVehicle;

public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, Guid>
{
    private readonly IVehicleWriteRepository _vehicleWriteRepository;
    private readonly IVehicleCatalogRepository _catalogRepository;
    private readonly IVehicleBlueprintRepository _blueprintRepository;
    private readonly IWorkspaceProvider _workspaceProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVehicleCommandHandler(
        IVehicleWriteRepository vehicleWriteRepository,
        IVehicleCatalogRepository catalogRepository,
        IVehicleBlueprintRepository blueprintRepository,
        IWorkspaceProvider workspaceProvider,
        IUnitOfWork unitOfWork)
    {
        _vehicleWriteRepository = vehicleWriteRepository;
        _catalogRepository = catalogRepository;
        _blueprintRepository = blueprintRepository;
        _workspaceProvider = workspaceProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var workspaceId = _workspaceProvider.WorkspaceId;
        var userId = _workspaceProvider.UserId;

        var brand = await _catalogRepository.EnsureBrandExistsAsync(request.Brand, cancellationToken);
        await _catalogRepository.EnsureModelExistsAsync(brand.Id, request.Model, request.Type, cancellationToken);
        await _catalogRepository.EnsureEngineExistsAsync(request.Engine, request.Type, cancellationToken);

        var vehicle = request.ToDomain(workspaceId!.Value, userId!.Value);

        await _vehicleWriteRepository.AddAsync(vehicle, cancellationToken);

        var existingBlueprint = await _blueprintRepository.FindMatchingBlueprintAsync(
            vehicle.Brand,
            vehicle.Model,
            vehicle.Engine,
            vehicle.Year,
            vehicle.Type,
            cancellationToken);

        if (existingBlueprint is null)
        {
            var blueprint = DotCruz.Garage.Domain.Entities.Vehicles.VehicleBlueprint.Create(
                vehicle.Brand,
                vehicle.Model,
                vehicle.Engine,
                vehicle.Type,
                vehicle.Year - 1,
                vehicle.Year + 3);

            await _blueprintRepository.AddAsync(blueprint, cancellationToken);
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return vehicle.Id;
    }
}
