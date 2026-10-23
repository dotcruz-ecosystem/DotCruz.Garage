using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.ServiceOrders;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.ServiceOrders.Commands.RemoveServiceOrderAttachment;

public class RemoveServiceOrderAttachmentCommandHandler : IRequestHandler<RemoveServiceOrderAttachmentCommand, Unit>
{
    private readonly IServiceOrderWriteRepository _serviceOrderWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveServiceOrderAttachmentCommandHandler(
        IServiceOrderWriteRepository serviceOrderWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _serviceOrderWriteRepository = serviceOrderWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(RemoveServiceOrderAttachmentCommand request, CancellationToken cancellationToken)
    {
        var serviceOrder = await _serviceOrderWriteRepository.GetByIdWithDetailsToUpdateAsync(request.ServiceOrderId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.UNKNOWN_ERROR);

        serviceOrder.RemoveAttachment(request.AttachmentId);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Unit.Value;
    }
}
