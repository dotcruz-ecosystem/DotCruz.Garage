using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Documents;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Documents.Commands.PayVehicleDocument;

public class PayVehicleDocumentCommandHandler : IRequestHandler<PayVehicleDocumentCommand>
{
    private readonly IVehicleDocumentWriteRepository _vehicleDocumentWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PayVehicleDocumentCommandHandler(
        IVehicleDocumentWriteRepository vehicleDocumentWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleDocumentWriteRepository = vehicleDocumentWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(PayVehicleDocumentCommand request, CancellationToken cancellationToken)
    {
        var doc = await _vehicleDocumentWriteRepository.GetByIdToUpdateAsync(request.DocumentId, cancellationToken) 
            ?? throw new NotFoundException(ResourceMessagesException.DOCUMENT_NOT_FOUND);
        
        doc.Pay(request.Request.FileUrl);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
