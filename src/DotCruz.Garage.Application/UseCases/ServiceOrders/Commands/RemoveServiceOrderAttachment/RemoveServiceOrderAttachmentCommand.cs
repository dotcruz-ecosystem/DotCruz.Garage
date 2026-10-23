using MediatR;

namespace DotCruz.Garage.Application.UseCases.ServiceOrders.Commands.RemoveServiceOrderAttachment;

public record RemoveServiceOrderAttachmentCommand(
    Guid ServiceOrderId,
    Guid AttachmentId) : IRequest<Unit>;
