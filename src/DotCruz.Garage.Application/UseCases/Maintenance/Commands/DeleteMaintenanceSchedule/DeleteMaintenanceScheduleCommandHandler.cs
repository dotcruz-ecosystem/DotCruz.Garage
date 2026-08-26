using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Commands.DeleteMaintenanceSchedule;

public class DeleteMaintenanceScheduleCommandHandler : IRequestHandler<DeleteMaintenanceScheduleCommand>
{
    private readonly IMaintenanceScheduleWriteRepository _maintenanceScheduleWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMaintenanceScheduleCommandHandler(
        IMaintenanceScheduleWriteRepository maintenanceScheduleWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _maintenanceScheduleWriteRepository = maintenanceScheduleWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteMaintenanceScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _maintenanceScheduleWriteRepository.GetByIdToUpdateAsync(request.ScheduleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.MAINTENANCE_SCHEDULE_NOT_FOUND);

        if (schedule.VehicleId != request.VehicleId)
            throw new NotFoundException(ResourceMessagesException.MAINTENANCE_SCHEDULE_NOT_FOUND);

        schedule.Delete();

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
