using DotCruz.Garage.Application.Abstractions.Data;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Fines;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.Fines.Commands.PayTrafficFine;

public class PayTrafficFineCommandHandler : IRequestHandler<PayTrafficFineCommand>
{
    private readonly ITrafficFineWriteRepository _trafficFineWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PayTrafficFineCommandHandler(
        ITrafficFineWriteRepository trafficFineWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _trafficFineWriteRepository = trafficFineWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(PayTrafficFineCommand request, CancellationToken cancellationToken)
    {
        var fine = await _trafficFineWriteRepository.GetByIdToUpdateAsync(request.FineId, cancellationToken) 
            ?? throw new NotFoundException(ResourceMessagesException.FINE_NOT_FOUND);
        
        fine.Pay(request.RequestDto.ReceiptUrl);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
