using CommonTestUtilities.Entities;
using CommonTestUtilities.Data;
using CommonTestUtilities.Repositories;
using DotCruz.Garage.Application.DTOs.Maintenance;
using DotCruz.Garage.Application.UseCases.Maintenance.Commands.AddMaintenanceAttachment;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;

namespace UseCases.Test.Maintenance;

public class AddMaintenanceAttachmentCommandHandlerTest
{
    [Fact]
    public async Task Successfully_Adds_Attachment_To_Existing_MaintenanceLog()
    {
        var log = MaintenanceLogBuilder.Build();

        var (_, writeRepo) = new MaintenanceLogRepositoryBuilder()
            .SetupGetByIdWithAttachmentsToUpdate(log)
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new AddMaintenanceAttachmentCommandHandler(writeRepo, unitOfWork);

        var command = new AddMaintenanceAttachmentCommand(
            log.VehicleId,
            log.Id,
            new CreateMaintenanceAttachmentDto(
                "nota_fiscal_pecas.pdf",
                "https://pub.r2.dev/nota_fiscal_pecas.pdf",
                "application/pdf",
                1_200_000));

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal("nota_fiscal_pecas.pdf", result.FileName);
        Assert.Equal("https://pub.r2.dev/nota_fiscal_pecas.pdf", result.FileUrl);
        Assert.Equal("application/pdf", result.ContentType);
        Assert.Equal(1_200_000, result.FileSize);
        Assert.Single(log.Attachments);
    }

    [Fact]
    public async Task Throws_NotFoundException_When_Log_Does_Not_Exist()
    {
        var (_, writeRepo) = new MaintenanceLogRepositoryBuilder()
            .SetupGetByIdWithAttachmentsToUpdate(null)
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var handler = new AddMaintenanceAttachmentCommandHandler(writeRepo, unitOfWork);

        var command = new AddMaintenanceAttachmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new CreateMaintenanceAttachmentDto("a.pdf", "https://url.com", "application/pdf", 100));

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, TestContext.Current.CancellationToken));
    }
}
