using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Drivers.Commands.DeleteDriver;

public class DeleteDriverCommandHandler : IRequestHandler<DeleteDriverCommand>
{
    private readonly IDriverWriteRepository _driverWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDriverCommandHandler(
        IDriverWriteRepository driverWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _driverWriteRepository = driverWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteDriverCommand request, CancellationToken cancellationToken)
    {
        var driver = await _driverWriteRepository.GetByIdToUpdateAsync(request.DriverId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.DRIVER_NOT_FOUND);

        driver.Delete();
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
