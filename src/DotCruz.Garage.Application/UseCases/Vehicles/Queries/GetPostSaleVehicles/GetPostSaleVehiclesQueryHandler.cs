using DotCruz.Garage.Application.Abstractions.Data.Repositories.Vehicles;
using DotCruz.Garage.Application.DTOs.Vehicles;
using DotCruz.Garage.Application.Mappers.Vehicles;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Vehicles.Queries.GetPostSaleVehicles;

public class GetPostSaleVehiclesQueryHandler : IRequestHandler<GetPostSaleVehiclesQuery, IReadOnlyCollection<VehicleSaleDto>>
{
    private readonly IVehicleSaleReadRepository _vehicleSaleReadRepository;

    public GetPostSaleVehiclesQueryHandler(IVehicleSaleReadRepository vehicleSaleReadRepository)
    {
        _vehicleSaleReadRepository = vehicleSaleReadRepository;
    }

    public async Task<IReadOnlyCollection<VehicleSaleDto>> Handle(GetPostSaleVehiclesQuery request, CancellationToken cancellationToken)
    {
        var sales = await _vehicleSaleReadRepository.GetActiveSalesWithVehicleAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;
        return sales
            .Select(s => s.ToDto(now: now))
            .ToList();
    }
}
