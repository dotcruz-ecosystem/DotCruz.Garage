using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Domain.Entities.Drivers;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Shared.Security.Context;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Drivers.Commands.AssignDriverToVehicle;

public class AssignDriverToVehicleCommandHandler : IRequestHandler<AssignDriverToVehicleCommand, Guid>
{
    private readonly IDriverReadRepository _driverReadRepository;
    private readonly IVehicleReadRepository _vehicleReadRepository;
    private readonly IVehicleDriverAssignmentWriteRepository _assignmentWriteRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly IUnitOfWork _unitOfWork;

    public AssignDriverToVehicleCommandHandler(
        IDriverReadRepository driverReadRepository,
        IVehicleReadRepository vehicleReadRepository,
        IVehicleDriverAssignmentWriteRepository assignmentWriteRepository,
        ITenantProvider tenantProvider,
        IUnitOfWork unitOfWork)
    {
        _driverReadRepository = driverReadRepository;
        _vehicleReadRepository = vehicleReadRepository;
        _assignmentWriteRepository = assignmentWriteRepository;
        _tenantProvider = tenantProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AssignDriverToVehicleCommand request, CancellationToken cancellationToken)
    {
        var driver = await _driverReadRepository.GetByIdAsync(request.DriverId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.DRIVER_NOT_FOUND);

        if (!driver.IsActive)
            throw new ErrorOnValidationException(ResourceMessagesException.DRIVER_INACTIVE);

        var vehicle = await _vehicleReadRepository.GetByIdAsync(request.RequestDto.VehicleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var startedAt = request.RequestDto.StartedAt ?? DateTimeOffset.UtcNow;

        var currentAssignment = await _assignmentWriteRepository.GetCurrentByVehicleIdToUpdateAsync(vehicle.Id, cancellationToken);
        if (currentAssignment != null)
        {
            if (currentAssignment.DriverId == driver.Id)
            {
                throw new ErrorOnValidationException(ResourceMessagesException.DRIVER_ALREADY_ASSIGNED);
            }
            currentAssignment.EndAssignment(startedAt);
        }

        var tenantId = _tenantProvider.TenantId ?? vehicle.TenantId;
        var userId = _tenantProvider.UserId ?? vehicle.UserId;
        var assignment = VehicleDriverAssignment.Create(
            tenantId,
            userId,
            vehicle.Id,
            driver.Id,
            startedAt,
            request.RequestDto.Notes);

        await _assignmentWriteRepository.AddAsync(assignment, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return assignment.Id;
    }
}
