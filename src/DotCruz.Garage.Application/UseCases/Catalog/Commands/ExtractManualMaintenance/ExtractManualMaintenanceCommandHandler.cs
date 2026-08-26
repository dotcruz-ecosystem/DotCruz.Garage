using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Catalog;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Abstractions.Services.Ai;
using DotCruz.Garage.Application.DTOs.Catalog;
using DotCruz.Garage.Application.Mappers.Catalog;
using DotCruz.Garage.Domain.Entities.Catalog;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Enums;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Catalog.Commands.ExtractManualMaintenance;

public class ExtractManualMaintenanceCommandHandler : IRequestHandler<ExtractManualMaintenanceCommand, VehicleBlueprintDto>
{
    private readonly IVehicleBlueprintReadRepository _blueprintReadRepository;
    private readonly IVehicleBlueprintWriteRepository _blueprintWriteRepository;
    private readonly IVehicleReadRepository _vehicleReadRepository;
    private readonly IMaintenanceScheduleWriteRepository _maintenanceScheduleWriteRepository;
    private readonly IAiManualParserService _aiManualParserService;
    private readonly IUnitOfWork _unitOfWork;

    public ExtractManualMaintenanceCommandHandler(
        IVehicleBlueprintReadRepository blueprintReadRepository,
        IVehicleBlueprintWriteRepository blueprintWriteRepository,
        IVehicleReadRepository vehicleReadRepository,
        IMaintenanceScheduleWriteRepository maintenanceScheduleWriteRepository,
        IAiManualParserService aiManualParserService,
        IUnitOfWork unitOfWork)
    {
        _blueprintReadRepository = blueprintReadRepository;
        _blueprintWriteRepository = blueprintWriteRepository;
        _vehicleReadRepository = vehicleReadRepository;
        _maintenanceScheduleWriteRepository = maintenanceScheduleWriteRepository;
        _aiManualParserService = aiManualParserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<VehicleBlueprintDto> Handle(ExtractManualMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var existingBlueprint = await _blueprintReadRepository.GetByDetailsWithTemplatesAsync(
            request.Brand,
            request.Model,
            request.Year,
            request.Engine,
            cancellationToken);

        VehicleBlueprint blueprint;

        if (existingBlueprint != null)
        {
            blueprint = existingBlueprint;
        }
        else
        {
            var extracted = await _aiManualParserService.ExtractMaintenancePlanAsync(
                request.FileStream,
                request.FileName,
                request.Brand,
                request.Model,
                request.Year,
                request.Engine,
                cancellationToken);

            blueprint = VehicleBlueprint.Create(
                request.Brand,
                request.Model,
                request.Year,
                request.Engine ?? extracted.Engine,
                request.Version ?? extracted.Version,
                BlueprintSource.AiExtraction);

            foreach (var templateDto in extracted.Templates)
            {
                var template = MaintenancePlanTemplate.Create(
                    blueprint.Id,
                    templateDto.Description,
                    templateDto.Category,
                    templateDto.IntervalOdometer,
                    templateDto.IntervalTimeMonths,
                    templateDto.Items,
                    templateDto.IsRecurring);

                blueprint.AddPlanTemplate(template);
            }

            await _blueprintWriteRepository.AddAsync(blueprint, cancellationToken);
        }

        if (request.VehicleId.HasValue)
        {
            var vehicle = await _vehicleReadRepository.GetByIdAsync(request.VehicleId.Value, cancellationToken);
            if (vehicle is null)
            {
                throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);
            }

            foreach (var template in blueprint.PlanTemplates)
            {
                var schedule = MaintenanceSchedule.Create(
                    tenantId: vehicle.TenantId,
                    userId: vehicle.UserId,
                    vehicleId: vehicle.Id,
                    description: template.Description,
                    category: template.Category,
                    intervalOdometer: template.IntervalOdometer,
                    intervalTimeMonths: template.IntervalTimeMonths,
                    lastPerformedOdometer: null,
                    lastPerformedDate: null,
                    warningThresholdOdometer: null,
                    warningThresholdDays: null,
                    currentOdometer: vehicle.CurrentOdometer,
                    dailyMileageAverage: vehicle.DailyMileageAverage);

                await _maintenanceScheduleWriteRepository.AddAsync(schedule, cancellationToken);
            }
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return blueprint.ToDto();
    }
}
