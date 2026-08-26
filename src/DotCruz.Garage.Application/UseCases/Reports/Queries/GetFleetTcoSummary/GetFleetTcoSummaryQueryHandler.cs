using DotCruz.Garage.Application.Abstractions.Data.Repositories.Accessories;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Documents;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Fines;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.DTOs.Reports;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Reports.Queries.GetFleetTcoSummary;

public class GetFleetTcoSummaryQueryHandler : IRequestHandler<GetFleetTcoSummaryQuery, FleetTcoSummaryDto>
{
    private readonly IVehicleReadRepository _vehicleReadRepository;
    private readonly IMaintenanceLogReadRepository _logReadRepository;
    private readonly ITrafficFineReadRepository _fineReadRepository;
    private readonly IVehicleDocumentReadRepository _documentReadRepository;
    private readonly IVehicleAccessoryReadRepository _accessoryReadRepository;

    public GetFleetTcoSummaryQueryHandler(
        IVehicleReadRepository vehicleReadRepository,
        IMaintenanceLogReadRepository logReadRepository,
        ITrafficFineReadRepository fineReadRepository,
        IVehicleDocumentReadRepository documentReadRepository,
        IVehicleAccessoryReadRepository accessoryReadRepository)
    {
        _vehicleReadRepository = vehicleReadRepository;
        _logReadRepository = logReadRepository;
        _fineReadRepository = fineReadRepository;
        _documentReadRepository = documentReadRepository;
        _accessoryReadRepository = accessoryReadRepository;
    }

    public async Task<FleetTcoSummaryDto> Handle(GetFleetTcoSummaryQuery request, CancellationToken cancellationToken)
    {
        var allVehicles = await _vehicleReadRepository.GetAllAsync(cancellationToken);
        var vehicles = request.VehicleId.HasValue
            ? allVehicles.Where(v => v.Id == request.VehicleId.Value).ToList()
            : allVehicles.ToList();

        var vehicleIds = vehicles.Select(v => v.Id).ToHashSet();

        var allLogs = await _logReadRepository.GetAllAsync(cancellationToken);
        var logs = allLogs.Where(l => vehicleIds.Contains(l.VehicleId)).ToList();

        var allFines = await _fineReadRepository.GetAllAsync(cancellationToken);
        var fines = allFines.Where(f => vehicleIds.Contains(f.VehicleId)).ToList();

        var allDocs = await _documentReadRepository.GetAllAsync(cancellationToken);
        var docs = allDocs.Where(d => vehicleIds.Contains(d.VehicleId)).ToList();

        var allAccessories = await _accessoryReadRepository.GetAllAsync(cancellationToken);
        var accessories = allAccessories.Where(a => vehicleIds.Contains(a.VehicleId)).ToList();

        if (request.StartDate.HasValue)
        {
            logs = logs.Where(l => l.PerformedAt >= request.StartDate.Value).ToList();
            fines = fines.Where(f => f.InfractionDate >= request.StartDate.Value).ToList();
            docs = docs.Where(d => d.DueDate.HasValue && d.DueDate.Value >= request.StartDate.Value).ToList();
            accessories = accessories.Where(a => a.PurchaseDate >= request.StartDate.Value).ToList();
        }

        if (request.EndDate.HasValue)
        {
            logs = logs.Where(l => l.PerformedAt <= request.EndDate.Value).ToList();
            fines = fines.Where(f => f.InfractionDate <= request.EndDate.Value).ToList();
            docs = docs.Where(d => d.DueDate.HasValue && d.DueDate.Value <= request.EndDate.Value).ToList();
            accessories = accessories.Where(a => a.PurchaseDate <= request.EndDate.Value).ToList();
        }

        var ranking = new List<VehicleTcoItemDto>();
        foreach (var v in vehicles)
        {
            var vLogs = logs.Where(l => l.VehicleId == v.Id).ToList();
            var vFines = fines.Where(f => f.VehicleId == v.Id).ToList();
            var vDocs = docs.Where(d => d.VehicleId == v.Id).ToList();
            var vAccessories = accessories.Where(a => a.VehicleId == v.Id).ToList();

            var mCost = vLogs.Sum(l => l.Cost);
            var fCost = vFines.Sum(f => f.Amount);
            var dCost = vDocs.Sum(d => d.Cost);
            var aCost = vAccessories.Sum(a => a.Cost);
            var vTotalCost = mCost + fCost + dCost + aCost;

            var initialOdo = v.OdometerHistories.OrderBy(h => h.RecordedAt).FirstOrDefault()?.OdometerValue ?? v.CurrentOdometer;
            var distance = Math.Max(0, v.CurrentOdometer - initialOdo);
            var costPerKm = distance > 0 ? Math.Round(vTotalCost / distance, 4) : 0m;

            ranking.Add(new VehicleTcoItemDto(
                v.Id,
                $"{v.Brand} {v.Model} ({v.Plate})",
                v.Plate ?? "",
                v.CurrentOdometer,
                distance,
                mCost,
                fCost,
                dCost,
                aCost,
                vTotalCost,
                costPerKm
            ));
        }

        ranking = ranking.OrderByDescending(r => r.TotalCost).ToList();

        var totalFleetDistance = ranking.Sum(r => r.DistanceTraveledKm);
        var totalMaintenance = ranking.Sum(r => r.MaintenanceCost);
        var totalFines = ranking.Sum(r => r.FinesCost);
        var totalDocs = ranking.Sum(r => r.DocumentsCost);
        var totalAccessories = ranking.Sum(r => r.AccessoriesCost);
        var totalFleetCost = totalMaintenance + totalFines + totalDocs + totalAccessories;
        var avgCostPerKm = totalFleetDistance > 0 ? Math.Round(totalFleetCost / totalFleetDistance, 4) : 0m;

        var monthlyEvolution = new List<MonthlyTcoDto>();
        var now = DateTimeOffset.UtcNow;
        for (int i = 5; i >= 0; i--)
        {
            var monthDate = now.AddMonths(-i);
            var year = monthDate.Year;
            var month = monthDate.Month;

            var mCost = logs.Where(l => l.PerformedAt.Year == year && l.PerformedAt.Month == month).Sum(l => l.Cost);
            var fCost = fines.Where(f => f.InfractionDate.Year == year && f.InfractionDate.Month == month).Sum(f => f.Amount);
            var dCost = docs.Where(d => d.DueDate.HasValue && d.DueDate.Value.Year == year && d.DueDate.Value.Month == month).Sum(d => d.Cost);
            var aCost = accessories.Where(a => a.PurchaseDate.Year == year && a.PurchaseDate.Month == month).Sum(a => a.Cost);

            monthlyEvolution.Add(new MonthlyTcoDto(
                year,
                month,
                mCost,
                fCost,
                dCost,
                aCost,
                mCost + fCost + dCost + aCost
            ));
        }

        return new FleetTcoSummaryDto(
            vehicles.Count,
            totalFleetDistance,
            totalFleetCost,
            avgCostPerKm,
            totalMaintenance,
            totalFines,
            totalDocs,
            totalAccessories,
            ranking,
            monthlyEvolution
        );
    }
}
