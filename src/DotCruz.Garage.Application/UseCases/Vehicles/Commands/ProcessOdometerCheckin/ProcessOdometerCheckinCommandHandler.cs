using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.Abstractions.Services.Security;
using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Garage.Domain.Services;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Commands.ProcessOdometerCheckin;

public class ProcessOdometerCheckinCommandHandler : IRequestHandler<ProcessOdometerCheckinCommand, OdometerCheckinResultDto>
{
    public const int MaxRealisticJumpKm = 20_000;

    private readonly ICheckinTokenService _tokenService;
    private readonly IVehicleWriteRepository _vehicleWriteRepository;
    private readonly IMaintenanceScheduleWriteRepository _maintenanceScheduleWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessOdometerCheckinCommandHandler(
        ICheckinTokenService tokenService,
        IVehicleWriteRepository vehicleWriteRepository,
        IMaintenanceScheduleWriteRepository maintenanceScheduleWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _tokenService = tokenService;
        _vehicleWriteRepository = vehicleWriteRepository;
        _maintenanceScheduleWriteRepository = maintenanceScheduleWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OdometerCheckinResultDto> Handle(ProcessOdometerCheckinCommand request, CancellationToken cancellationToken)
    {
        if (!_tokenService.TryValidateToken(request.Token, out var checkinToken) || checkinToken is null)
        {
            throw new ErrorOnValidationException(ResourceMessagesException.TOKEN_INVALID);
        }

        if (checkinToken.IsExpired())
        {
            throw new ErrorOnValidationException(ResourceMessagesException.TOKEN_EXPIRED);
        }

        var vehicle = await _vehicleWriteRepository.GetByIdWithHistoriesToUpdateUnscopedAsync(
            checkinToken.VehicleId,
            checkinToken.WorkspaceId,
            cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var recordedAt = request.RecordedAt ?? DateTimeOffset.UtcNow;
        var oldOdometer = vehicle.CurrentOdometer;

        if (request.NewOdometer < oldOdometer)
        {
            throw new ErrorOnValidationException(ResourceMessagesException.ODOMETER_VALUE_INVALID);
        }

        if (request.NewOdometer - oldOdometer > MaxRealisticJumpKm)
        {
            throw new ErrorOnValidationException(ResourceMessagesException.ODOMETER_UNREALISTIC_JUMP);
        }

        vehicle.UpdateOdometer(request.NewOdometer, recordedAt);
        var history = OdometerHistory.Create(vehicle.Id, request.NewOdometer, recordedAt);
        vehicle.OdometerHistories.Add(history);

        var newAverage = OdometerCalculator.CalculateDailyAverage(
            vehicle.OdometerHistories,
            vehicle.DailyMileageAverage);
        vehicle.UpdateDailyMileageAverage(newAverage);

        var schedules = await _maintenanceScheduleWriteRepository.GetByVehicleIdToUpdateUnscopedAsync(
            vehicle.Id,
            checkinToken.WorkspaceId,
            cancellationToken);

        foreach (var schedule in schedules)
        {
            schedule.RecalculateNextDue(vehicle.CurrentOdometer, vehicle.DailyMileageAverage);
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return new OdometerCheckinResultDto(
            vehicle.Id,
            vehicle.Name,
            oldOdometer,
            request.NewOdometer,
            recordedAt,
            schedules.Count > 0);
    }
}
