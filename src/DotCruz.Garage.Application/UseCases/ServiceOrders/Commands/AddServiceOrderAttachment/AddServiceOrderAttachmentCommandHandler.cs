using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.ServiceOrders;
using DotCruz.Garage.Application.DTOs.ServiceOrders;
using DotCruz.Garage.Application.Mappers.ServiceOrders;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.ServiceOrders.Commands.AddServiceOrderAttachment;

public class AddServiceOrderAttachmentCommandHandler : IRequestHandler<AddServiceOrderAttachmentCommand, ServiceOrderAttachmentDto>
{
    private readonly IServiceOrderWriteRepository _serviceOrderWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddServiceOrderAttachmentCommandHandler(
        IServiceOrderWriteRepository serviceOrderWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _serviceOrderWriteRepository = serviceOrderWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceOrderAttachmentDto> Handle(AddServiceOrderAttachmentCommand request, CancellationToken cancellationToken)
    {
        var serviceOrder = await _serviceOrderWriteRepository.GetByIdWithDetailsToUpdateAsync(request.ServiceOrderId, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.UNKNOWN_ERROR);

        var attachment = serviceOrder.AddAttachment(
            request.FileName,
            request.FileUrl,
            request.ContentType,
            request.FileSize);

        await _unitOfWork.CommitAsync(cancellationToken);

        return attachment.ToDto();
    }
}
