using System.Security.Cryptography;
using System.Text;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Accessories;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Documents;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Fines;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.DTOs.Reports;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Reports.Queries.GetVehiclePassport;

public class GetVehiclePassportQueryHandler : IRequestHandler<GetVehiclePassportQuery, VehiclePassportDto>
{
    private readonly IVehicleReadRepository _vehicleReadRepository;
    private readonly IVehicleDriverAssignmentReadRepository _assignmentReadRepository;
    private readonly IMaintenanceLogReadRepository _logReadRepository;
    private readonly IMaintenanceScheduleReadRepository _scheduleReadRepository;
    private readonly IVehicleDocumentReadRepository _documentReadRepository;
    private readonly ITrafficFineReadRepository _fineReadRepository;
    private readonly IVehicleAccessoryReadRepository _accessoryReadRepository;

    public GetVehiclePassportQueryHandler(
        IVehicleReadRepository vehicleReadRepository,
        IVehicleDriverAssignmentReadRepository assignmentReadRepository,
        IMaintenanceLogReadRepository logReadRepository,
        IMaintenanceScheduleReadRepository scheduleReadRepository,
        IVehicleDocumentReadRepository documentReadRepository,
        ITrafficFineReadRepository fineReadRepository,
        IVehicleAccessoryReadRepository accessoryReadRepository)
    {
        _vehicleReadRepository = vehicleReadRepository;
        _assignmentReadRepository = assignmentReadRepository;
        _logReadRepository = logReadRepository;
        _scheduleReadRepository = scheduleReadRepository;
        _documentReadRepository = documentReadRepository;
        _fineReadRepository = fineReadRepository;
        _accessoryReadRepository = accessoryReadRepository;
    }

    public async Task<VehiclePassportDto> Handle(GetVehiclePassportQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleReadRepository.GetByIdAsync(request.VehicleId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.VEHICLE_NOT_FOUND);

        var currentAssignment = await _assignmentReadRepository.GetCurrentByVehicleIdAsync(vehicle.Id, cancellationToken);
        var (logs, _) = await _logReadRepository.GetByVehicleIdPaginatedAsync(vehicle.Id, 1, 1000, cancellationToken);
        var schedules = await _scheduleReadRepository.GetByVehicleIdAsync(vehicle.Id, cancellationToken);
        var documents = await _documentReadRepository.GetByVehicleIdAsync(vehicle.Id, cancellationToken);
        var fines = await _fineReadRepository.GetByVehicleIdAsync(vehicle.Id, cancellationToken);
        var accessories = await _accessoryReadRepository.GetByVehicleIdAsync(vehicle.Id, cancellationToken);

        var totalMaintenance = logs.Sum(l => l.Cost);
        var totalFines = fines.Sum(f => f.Amount);
        var totalDocs = documents.Sum(d => d.Cost);
        var totalAccessories = accessories.Sum(a => a.Cost);
        var totalTco = totalMaintenance + totalFines + totalDocs + totalAccessories;

        var initialOdo = vehicle.OdometerHistories.OrderBy(h => h.RecordedAt).FirstOrDefault()?.OdometerValue ?? vehicle.CurrentOdometer;
        var distance = Math.Max(0, vehicle.CurrentOdometer - initialOdo);
        var costPerKm = distance > 0 ? Math.Round(totalTco / distance, 4) : 0m;

        var financialSummary = new VehicleFinancialSummaryDto(
            totalMaintenance,
            totalFines,
            totalDocs,
            totalAccessories,
            totalTco,
            costPerKm
        );

        var passportLogs = logs
            .OrderByDescending(l => l.PerformedAt)
            .Select(l => new PassportMaintenanceLogDto(
                l.Id,
                l.PerformedAt,
                l.OdometerAtService,
                l.Description,
                l.Cost,
                l.ReceiptUrl,
                l.Notes
            ))
            .ToList();

        var passportSchedules = schedules
            .OrderBy(s => s.NextDueOdometer)
            .Select(s => new PassportMaintenanceScheduleDto(
                s.Id,
                s.Description,
                s.Category.ToString(),
                s.NextDueOdometer,
                s.NextDueDate,
                s.Status.ToString()
            ))
            .ToList();

        var passportDocs = documents
            .OrderByDescending(d => d.ReferenceYear)
            .Select(d => new PassportDocumentDto(
                d.Id,
                d.Type.ToString(),
                d.ReferenceYear,
                d.Cost,
                d.ExpirationDate,
                d.Status.ToString()
            ))
            .ToList();

        var generatedAt = DateTimeOffset.UtcNow;
        var verificationRaw = $"{vehicle.Id}-{vehicle.Plate}-{vehicle.CurrentOdometer}-{generatedAt.Ticks}";
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(verificationRaw));
        var verificationCode = $"DC-PASSPORT-{Convert.ToHexString(hashBytes)[..12]}";

        return new VehiclePassportDto(
            vehicle.Id,
            vehicle.Name,
            vehicle.Brand,
            vehicle.Model,
            vehicle.Year,
            vehicle.Plate,
            vehicle.Vin,
            vehicle.CurrentOdometer,
            distance,
            vehicle.DailyMileageAverage,
            currentAssignment?.Driver?.Name,
            financialSummary,
            passportLogs,
            passportSchedules,
            passportDocs,
            generatedAt,
            verificationCode
        );
    }
}
