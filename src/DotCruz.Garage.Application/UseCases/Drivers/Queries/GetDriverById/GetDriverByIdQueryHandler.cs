using DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;
using DotCruz.Garage.Application.DTOs.Drivers;
using DotCruz.Garage.Application.Mappers.Drivers;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Drivers.Queries.GetDriverById;

public class GetDriverByIdQueryHandler : IRequestHandler<GetDriverByIdQuery, DriverDto>
{
    private readonly IDriverReadRepository _driverReadRepository;

    public GetDriverByIdQueryHandler(IDriverReadRepository driverReadRepository)
    {
        _driverReadRepository = driverReadRepository;
    }

    public async Task<DriverDto> Handle(GetDriverByIdQuery request, CancellationToken cancellationToken)
    {
        var driver = await _driverReadRepository.GetByIdWithFinesAsync(request.DriverId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.DRIVER_NOT_FOUND);

        return driver.ToDto();
    }
}
