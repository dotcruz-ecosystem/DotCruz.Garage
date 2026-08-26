using DotCruz.Garage.Application.Abstractions.Data.Repositories.Fines;
using DotCruz.Garage.Application.DTOs.Fines;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Fines.Queries.GetPendingNICFines;

public class GetPendingNICFinesQueryHandler : IRequestHandler<GetPendingNICFinesQuery, IReadOnlyCollection<NICFineAlertDto>>
{
    private readonly ITrafficFineReadRepository _fineReadRepository;

    public GetPendingNICFinesQueryHandler(ITrafficFineReadRepository fineReadRepository)
    {
        _fineReadRepository = fineReadRepository;
    }

    public async Task<IReadOnlyCollection<NICFineAlertDto>> Handle(GetPendingNICFinesQuery request, CancellationToken cancellationToken)
    {
        var fines = await _fineReadRepository.GetPendingNICFinesAsync(cancellationToken);

        return fines
            .Where(f => f.IsNICRisk())
            .OrderBy(f => f.IndicationDeadline)
            .Select(f =>
            {
                var days = f.DaysUntilIndicationDeadline();
                return new NICFineAlertDto(
                    f.Id,
                    f.VehicleId,
                    f.Vehicle?.Name ?? "Veículo",
                    f.InfractionCode,
                    f.Description,
                    f.Amount,
                    f.InfractionDate,
                    f.IndicationDeadline,
                    days,
                    days.HasValue && days.Value <= 7
                );
            })
            .ToList();
    }
}
