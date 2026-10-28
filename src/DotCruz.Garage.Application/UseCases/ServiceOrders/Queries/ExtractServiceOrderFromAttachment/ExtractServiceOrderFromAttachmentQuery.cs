using DotCruz.Garage.Application.DTOs.ServiceOrders;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.ServiceOrders.Queries.ExtractServiceOrderFromAttachment;

public record ExtractServiceOrderFromAttachmentQuery(
    Guid VehicleId,
    ExtractServiceOrderFromAttachmentRequestDto RequestDto) : IRequest<ExtractedServiceOrderDto>;
