using DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;
using DotCruz.Garage.Application.DTOs.Drivers;
using DotCruz.Garage.Application.Mappers.Drivers;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Drivers.Queries.GetDriverFinesSummary;

public class GetDriverFinesSummaryQueryHandler : IRequestHandler<GetDriverFinesSummaryQuery, DriverFinesSummaryDto>
{
    private readonly IDriverReadRepository _driverReadRepository;

    public GetDriverFinesSummaryQueryHandler(IDriverReadRepository driverReadRepository)
    {
        _driverReadRepository = driverReadRepository;
    }

    public async Task<DriverFinesSummaryDto> Handle(GetDriverFinesSummaryQuery request, CancellationToken cancellationToken)
    {
        var driver = await _driverReadRepository.GetByIdWithFinesAsync(request.DriverId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.DRIVER_NOT_FOUND);

        return driver.ToSummaryDto();
    }
}
