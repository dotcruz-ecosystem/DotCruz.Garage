using DotCruz.Garage.Application.Abstractions.Data.Repositories.ServiceOrders;
using DotCruz.Garage.Application.DTOs.ServiceOrders;
using DotCruz.Garage.Application.Mappers.ServiceOrders;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.ServiceOrders.Queries.GetServiceOrderById;

public class GetServiceOrderByIdQueryHandler : IRequestHandler<GetServiceOrderByIdQuery, ServiceOrderDto>
{
    private readonly IServiceOrderReadRepository _serviceOrderReadRepository;

    public GetServiceOrderByIdQueryHandler(IServiceOrderReadRepository serviceOrderReadRepository)
    {
        _serviceOrderReadRepository = serviceOrderReadRepository;
    }

    public async Task<ServiceOrderDto> Handle(GetServiceOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _serviceOrderReadRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(ResourceMessagesException.UNKNOWN_ERROR);

        return order.ToDto();
    }
}
