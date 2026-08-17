using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Accessories;
using DotCruz.Shared.Security.Context;
using DotCruz.Garage.Application.Mappers.Accessories;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Accessories.Commands.CreateVehicleAccessory;

public class CreateVehicleAccessoryCommandHandler : IRequestHandler<CreateVehicleAccessoryCommand, Guid>
{
    private readonly IVehicleReadRepository _vehicleReadRepository;
    private readonly IVehicleAccessoryWriteRepository _vehicleAccessoryWriteRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVehicleAccessoryCommandHandler(
        IVehicleReadRepository vehicleReadRepository,
        IVehicleAccessoryWriteRepository vehicleAccessoryWriteRepository,
        ITenantProvider tenantProvider,
        IUnitOfWork unitOfWork)
    {
        _vehicleReadRepository = vehicleReadRepository;
        _vehicleAccessoryWriteRepository = vehicleAccessoryWriteRepository;
        _tenantProvider = tenantProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateVehicleAccessoryCommand request, CancellationToken cancellationToken)
    {
        var vehicleExists = await _vehicleReadRepository.ExistsAsync(request.VehicleId, cancellationToken);
        if (!vehicleExists)
            throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var tenantId = _tenantProvider.TenantId;
        var userId = _tenantProvider.UserId;

        var vehicleAccessory = request.ToDomain(tenantId!.Value, userId!.Value);

        await _vehicleAccessoryWriteRepository.AddAsync(vehicleAccessory, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return vehicleAccessory.Id;
    }
}
