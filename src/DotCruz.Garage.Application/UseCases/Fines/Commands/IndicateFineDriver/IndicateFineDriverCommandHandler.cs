using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Fines;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Fines.Commands.IndicateFineDriver;

public class IndicateFineDriverCommandHandler : IRequestHandler<IndicateFineDriverCommand>
{
    private readonly ITrafficFineWriteRepository _fineWriteRepository;
    private readonly IDriverReadRepository _driverReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IndicateFineDriverCommandHandler(
        ITrafficFineWriteRepository fineWriteRepository,
        IDriverReadRepository driverReadRepository,
        IUnitOfWork unitOfWork)
    {
        _fineWriteRepository = fineWriteRepository;
        _driverReadRepository = driverReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(IndicateFineDriverCommand request, CancellationToken cancellationToken)
    {
        var fine = await _fineWriteRepository.GetByIdToUpdateAsync(request.FineId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.FINE_NOT_FOUND);

        var driver = await _driverReadRepository.GetByIdAsync(request.RequestDto.DriverId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.DRIVER_NOT_FOUND);

        if (!driver.IsActive)
            throw new ErrorOnValidationException(ResourceMessagesException.DRIVER_INACTIVE);

        if (fine.IndicationDeadline.HasValue && fine.IndicationDeadline.Value < DateTimeOffset.UtcNow)
        {
            throw new ErrorOnValidationException(ResourceMessagesException.INDICATION_DEADLINE_PASSED);
        }

        fine.IndicateDriver(driver.Id, request.RequestDto.Protocol);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
