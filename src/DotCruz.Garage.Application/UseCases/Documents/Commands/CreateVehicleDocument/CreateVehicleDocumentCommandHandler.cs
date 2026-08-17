using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Documents;
using DotCruz.Shared.Security.Context;
using DotCruz.Garage.Application.Mappers.Documents;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Documents.Commands.CreateVehicleDocument;

public class CreateVehicleDocumentCommandHandler : IRequestHandler<CreateVehicleDocumentCommand, Guid>
{
    private readonly IVehicleReadRepository _vehicleReadRepository;
    private readonly IVehicleDocumentWriteRepository _vehicleDocumentWriteRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVehicleDocumentCommandHandler(
        IVehicleReadRepository vehicleReadRepository,
        IVehicleDocumentWriteRepository vehicleDocumentWriteRepository,
        ITenantProvider tenantProvider,
        IUnitOfWork unitOfWork)
    {
        _vehicleReadRepository = vehicleReadRepository;
        _vehicleDocumentWriteRepository = vehicleDocumentWriteRepository;
        _tenantProvider = tenantProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateVehicleDocumentCommand request, CancellationToken cancellationToken)
    {
        var vehicleExists = await _vehicleReadRepository.ExistsAsync(request.VehicleId, cancellationToken);
        if (!vehicleExists)
            throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var tenantId = _tenantProvider.TenantId;
        var userId = _tenantProvider.UserId;

        var doc = request.ToDomain(tenantId!.Value, userId!.Value);

        await _vehicleDocumentWriteRepository.AddAsync(doc, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return doc.Id;
    }
}
