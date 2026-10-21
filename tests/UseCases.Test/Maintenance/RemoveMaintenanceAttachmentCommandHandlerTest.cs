using CommonTestUtilities.Entities;
using CommonTestUtilities.Data;
using CommonTestUtilities.Repositories;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.RemoveMaintenanceAttachment;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;

namespace UseCases.Test.Maintenance;

public class RemoveMaintenanceAttachmentCommandHandlerTest
{
    [Fact]
    public async Task Successfully_Removes_Attachment_From_MaintenanceLog()
    {
        var log = MaintenanceLogBuilder.Build();
        var attachment = log.AddAttachment("nota.pdf", "https://url.com/nota.pdf", "application/pdf", 500);

        var (_, writeRepo) = new MaintenanceLogRepositoryBuilder()
            .SetupGetByIdWithAttachmentsToUpdate(log)
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new RemoveMaintenanceAttachmentCommandHandler(writeRepo, unitOfWork);

        var command = new RemoveMaintenanceAttachmentCommand(log.VehicleId, log.Id, attachment.Id);

        await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.Empty(log.Attachments);
    }

    [Fact]
    public async Task Throws_NotFoundException_When_Attachment_Does_Not_Exist_On_Log()
    {
        var log = MaintenanceLogBuilder.Build();

        var (_, writeRepo) = new MaintenanceLogRepositoryBuilder()
            .SetupGetByIdWithAttachmentsToUpdate(log)
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new RemoveMaintenanceAttachmentCommandHandler(writeRepo, unitOfWork);

        var command = new RemoveMaintenanceAttachmentCommand(log.VehicleId, log.Id, Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, TestContext.Current.CancellationToken));
    }
}
