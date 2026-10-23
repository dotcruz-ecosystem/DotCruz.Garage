using DotCruz.Garage.Application.DTOs.ServiceOrders;
using MediatR;

namespace DotCruz.Garage.Application.UseCases.ServiceOrders.Commands.AddServiceOrderAttachment;

public record AddServiceOrderAttachmentCommand(
    Guid ServiceOrderId,
    string FileName,
    string FileUrl,
    string ContentType,
    long FileSize) : IRequest<ServiceOrderAttachmentDto>;
