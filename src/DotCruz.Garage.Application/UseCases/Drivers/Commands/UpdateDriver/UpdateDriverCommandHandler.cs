using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Drivers.Commands.UpdateDriver;

public class UpdateDriverCommandHandler : IRequestHandler<UpdateDriverCommand>
{
    private readonly IDriverWriteRepository _driverWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDriverCommandHandler(
        IDriverWriteRepository driverWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _driverWriteRepository = driverWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateDriverCommand request, CancellationToken cancellationToken)
    {
        var driver = await _driverWriteRepository.GetByIdToUpdateAsync(request.DriverId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.DRIVER_NOT_FOUND);

        driver.Update(
            request.RequestDto.Name,
            request.RequestDto.Cpf,
            request.RequestDto.Cnh,
            request.RequestDto.CnhCategory,
            request.RequestDto.CnhExpirationDate,
            request.RequestDto.Email,
            request.RequestDto.Phone
        );

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
