using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Drivers;
using DotCruz.Garage.Application.Mappers.Drivers;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using DotCruz.Shared.Security.Context;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Drivers.Commands.RegisterDriver;

public class RegisterDriverCommandHandler : IRequestHandler<RegisterDriverCommand, Guid>
{
    private readonly IDriverWriteRepository _driverWriteRepository;
    private readonly IDriverReadRepository _driverReadRepository;
    private readonly IWorkspaceProvider _workspaceProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterDriverCommandHandler(
        IDriverWriteRepository driverWriteRepository,
        IDriverReadRepository driverReadRepository,
        IWorkspaceProvider workspaceProvider,
        IUnitOfWork unitOfWork)
    {
        _driverWriteRepository = driverWriteRepository;
        _driverReadRepository = driverReadRepository;
        _workspaceProvider = workspaceProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegisterDriverCommand request, CancellationToken cancellationToken)
    {
        var existing = await _driverReadRepository.GetByCpfAsync(request.RequestDto.Cpf, cancellationToken);
        if (existing != null)
        {
            throw new ErrorOnValidationException(ResourceMessagesException.DRIVER_CPF_INVALID);
        }

        var workspaceId = _workspaceProvider.WorkspaceId ?? Guid.Empty;
        var userId = _workspaceProvider.UserId ?? Guid.Empty;
        var driver = request.ToDomain(workspaceId, userId);

        await _driverWriteRepository.AddAsync(driver, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return driver.Id;
    }
}
