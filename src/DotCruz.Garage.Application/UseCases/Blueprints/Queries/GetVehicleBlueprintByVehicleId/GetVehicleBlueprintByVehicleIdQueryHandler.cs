using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.DTOs.Blueprints;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Blueprints.Queries.GetVehicleBlueprintByVehicleId;

public class GetVehicleBlueprintByVehicleIdQueryHandler : IRequestHandler<GetVehicleBlueprintByVehicleIdQuery, VehicleBlueprintDto?>
{
    private readonly IVehicleReadRepository _vehicleReadRepository;
    private readonly IVehicleBlueprintRepository _blueprintRepository;
    private readonly IMaintenanceScheduleReadRepository _scheduleReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetVehicleBlueprintByVehicleIdQueryHandler(
        IVehicleReadRepository vehicleReadRepository,
        IVehicleBlueprintRepository blueprintRepository,
        IMaintenanceScheduleReadRepository scheduleReadRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleReadRepository = vehicleReadRepository;
        _blueprintRepository = blueprintRepository;
        _scheduleReadRepository = scheduleReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<VehicleBlueprintDto?> Handle(GetVehicleBlueprintByVehicleIdQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleReadRepository.GetByIdAsync(request.VehicleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var blueprint = await _blueprintRepository.FindMatchingBlueprintAsync(
            vehicle.Brand,
            vehicle.Model,
            vehicle.Engine,
            vehicle.Year,
            vehicle.Type,
            cancellationToken);

        if (blueprint is null)
        {
            var yearFrom = vehicle.Year;
            var yearTo = vehicle.Year;

            var newBlueprint = VehicleBlueprint.Create(
                vehicle.Brand,
                vehicle.Model,
                vehicle.Engine,
                vehicle.Type,
                yearFrom,
                yearTo);

            await _blueprintRepository.AddAsync(newBlueprint, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            blueprint = newBlueprint;
        }

        var existingSchedules = await _scheduleReadRepository.GetByVehicleIdAsync(request.VehicleId, cancellationToken);
        var existingDescriptions = existingSchedules
            .Select(s => s.Description.Trim().ToLower())
            .ToHashSet();

        var templateDtos = blueprint.Templates.Select(t => new MaintenancePlanTemplateDto(
            t.Id,
            t.Description,
            t.Category,
            t.IntervalOdometer,
            t.IntervalTimeMonths,
            t.IsRequired,
            existingDescriptions.Contains(t.Description.Trim().ToLower()),
            t.Checkpoints.OrderBy(c => c.Order).Select(c => new MaintenancePlanTemplateCheckpointDto(
                c.Id,
                c.Odometer,
                c.Months,
                c.Order
            )).ToList()
        )).ToList();

        return new VehicleBlueprintDto(
            blueprint.Id,
            blueprint.Brand,
            blueprint.Model,
            blueprint.Engine,
            blueprint.Type,
            blueprint.YearFrom,
            blueprint.YearTo,
            blueprint.SourceUrl,
            blueprint.Status,
            blueprint.ErrorMessage,
            templateDtos
        );
    }
}
