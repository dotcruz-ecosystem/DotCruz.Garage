using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.DTOs.Maintenance;
using DotCruz.Garage.Application.Mappers.Maintenance;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Maintenance.Queries.GetMaintenanceScheduleById;

public class GetMaintenanceScheduleByIdQueryHandler : IRequestHandler<GetMaintenanceScheduleByIdQuery, MaintenanceScheduleDto>
{
    private readonly IMaintenanceScheduleReadRepository _scheduleReadRepository;

    public GetMaintenanceScheduleByIdQueryHandler(IMaintenanceScheduleReadRepository scheduleReadRepository)
    {
        _scheduleReadRepository = scheduleReadRepository;
    }

    public async Task<MaintenanceScheduleDto> Handle(GetMaintenanceScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleReadRepository.GetByIdAsync(request.ScheduleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.MAINTENANCE_SCHEDULE_NOT_FOUND);

        if (schedule.VehicleId != request.VehicleId)
            throw new NotFoundException(ResourceMessagesException.MAINTENANCE_SCHEDULE_NOT_FOUND);

        return schedule.ToDto();
    }
}
