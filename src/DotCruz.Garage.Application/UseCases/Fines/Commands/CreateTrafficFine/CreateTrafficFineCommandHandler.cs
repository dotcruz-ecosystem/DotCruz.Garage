using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Fines;
using DotCruz.Shared.Security.Context;
using DotCruz.Garage.Application.Mappers.Fines;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Fines.Commands.CreateTrafficFine;

public class CreateTrafficFineCommandHandler : IRequestHandler<CreateTrafficFineCommand, Guid>
{
    private readonly IVehicleReadRepository _vehicleReadRepository;
    private readonly ITrafficFineWriteRepository _trafficFineWriteRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTrafficFineCommandHandler(
        IVehicleReadRepository vehicleReadRepository,
        ITrafficFineWriteRepository trafficFineWriteRepository,
        ITenantProvider tenantProvider,
        IUnitOfWork unitOfWork)
    {
        _vehicleReadRepository = vehicleReadRepository;
        _trafficFineWriteRepository = trafficFineWriteRepository;
        _tenantProvider = tenantProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateTrafficFineCommand request, CancellationToken cancellationToken)
    {
        var vehicleExists = await _vehicleReadRepository.ExistsAsync(request.VehicleId, cancellationToken);
        if (!vehicleExists)
            throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var tenantId = _tenantProvider.TenantId;
        var userId = _tenantProvider.UserId;

        var fine = request.ToDomain(tenantId!.Value, userId!.Value);

        await _trafficFineWriteRepository.AddAsync(fine, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return fine.Id;
    }
}
