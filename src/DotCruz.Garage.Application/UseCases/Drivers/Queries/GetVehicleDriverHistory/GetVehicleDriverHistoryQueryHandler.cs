using DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;
using DotCruz.Garage.Application.DTOs.Drivers;
using DotCruz.Garage.Application.Mappers.Drivers;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Drivers.Queries.GetVehicleDriverHistory;

public class GetVehicleDriverHistoryQueryHandler : IRequestHandler<GetVehicleDriverHistoryQuery, IReadOnlyCollection<VehicleDriverAssignmentDto>>
{
    private readonly IVehicleDriverAssignmentReadRepository _assignmentReadRepository;

    public GetVehicleDriverHistoryQueryHandler(IVehicleDriverAssignmentReadRepository assignmentReadRepository)
    {
        _assignmentReadRepository = assignmentReadRepository;
    }

    public async Task<IReadOnlyCollection<VehicleDriverAssignmentDto>> Handle(GetVehicleDriverHistoryQuery request, CancellationToken cancellationToken)
    {
        var assignments = await _assignmentReadRepository.GetByVehicleIdAsync(request.VehicleId, cancellationToken);
        return assignments.Select(a => a.ToDto()).ToList();
    }
}
