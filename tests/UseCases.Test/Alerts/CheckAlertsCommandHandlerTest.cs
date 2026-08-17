using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Documents;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Fines;
using DotCruz.Garage.Application.Abstractions.Data.Repositories.Maintenance;
using DotCruz.Garage.Application.Abstractions.Services.Auth.DTOs;
using DotCruz.Garage.Application.UseCases.Alerts.Commands.CheckAlerts;
using DotCruz.Garage.Domain.Entities.Documents;
using DotCruz.Garage.Domain.Entities.Fines;
using DotCruz.Garage.Domain.Entities.Maintenance;
using Moq;

namespace UseCases.Test.Alerts;

public class CheckAlertsCommandHandlerTest
{
    [Fact]
    public async Task Notifies_Each_Tenant_With_Its_Own_Tenant_Id()
    {
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        var scheduleA = DueSchedule(tenantA);
        var scheduleB = DueSchedule(tenantB);

        var authClient = new AuthServiceClientBuilder()
            .SetupAnyUser(new UserResponseDto(Guid.NewGuid(), "Tester", "tester@dotcruz.com"))
            .Build();
        var notifications = new NotificationServiceBuilder();

        var handler = CreateHandler([scheduleA, scheduleB], authClient, notifications);

        await handler.Handle(new CheckAlertsCommand(), TestContext.Current.CancellationToken);

        notifications.Mock.Verify(n => n.SendMaintenanceAlertAsync(
            scheduleA.UserId, tenantA, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()),
            Times.Once);

        notifications.Mock.Verify(n => n.SendMaintenanceAlertAsync(
            scheduleB.UserId, tenantB, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Resolves_The_User_Within_The_Record_Tenant()
    {
        var tenantId = Guid.NewGuid();
        var schedule = DueSchedule(tenantId);

        var authClient = new AuthServiceClientBuilder()
            .SetupAnyUser(new UserResponseDto(Guid.NewGuid(), "Tester", "tester@dotcruz.com"));

        var handler = CreateHandler([schedule], authClient.Build(), new NotificationServiceBuilder());

        await handler.Handle(new CheckAlertsCommand(), TestContext.Current.CancellationToken);

        authClient.Mock.Verify(
            c => c.GetUserByIdAsync(schedule.UserId, tenantId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Skips_The_Alert_When_The_User_Cannot_Be_Resolved()
    {
        var schedule = DueSchedule(Guid.NewGuid());

        var authClient = new AuthServiceClientBuilder()
            .SetupGetUserById(schedule.UserId, schedule.TenantId, null)
            .Build();
        var notifications = new NotificationServiceBuilder();

        var handler = CreateHandler([schedule], authClient, notifications);

        await handler.Handle(new CheckAlertsCommand(), TestContext.Current.CancellationToken);

        notifications.Mock.VerifyNoOtherCalls();
        Assert.Null(schedule.LastNotifiedAt);
    }

    [Fact]
    public async Task Marks_The_Schedule_As_Notified()
    {
        var schedule = DueSchedule(Guid.NewGuid());

        var authClient = new AuthServiceClientBuilder()
            .SetupAnyUser(new UserResponseDto(Guid.NewGuid(), "Tester", "tester@dotcruz.com"))
            .Build();

        var handler = CreateHandler([schedule], authClient, new NotificationServiceBuilder());

        await handler.Handle(new CheckAlertsCommand(), TestContext.Current.CancellationToken);

        Assert.NotNull(schedule.LastNotifiedAt);
        Assert.Equal(schedule.Status, schedule.LastNotifiedStatus);
    }

    [Fact]
    public async Task Resolves_Each_User_Once_Even_Across_Records()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var first = DueSchedule(tenantId, userId);
        var second = DueSchedule(tenantId, userId);

        var authClient = new AuthServiceClientBuilder()
            .SetupAnyUser(new UserResponseDto(userId, "Tester", "tester@dotcruz.com"));

        var handler = CreateHandler([first, second], authClient.Build(), new NotificationServiceBuilder());

        await handler.Handle(new CheckAlertsCommand(), TestContext.Current.CancellationToken);

        authClient.Mock.Verify(
            c => c.GetUserByIdAsync(userId, tenantId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Processes_One_Record_At_A_Time()
    {
        var tenantId = Guid.NewGuid();
        var schedules = Enumerable.Range(0, 5).Select(_ => DueSchedule(tenantId)).ToList();

        var inFlight = 0;
        var peak = 0;

        var notifications = new NotificationServiceBuilder();
        notifications.Mock
            .Setup(n => n.SendMaintenanceAlertAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                var current = Interlocked.Increment(ref inFlight);
                peak = Math.Max(peak, current);

                await Task.Delay(15);

                Interlocked.Decrement(ref inFlight);
            });

        var authClient = new AuthServiceClientBuilder()
            .SetupAnyUser(new UserResponseDto(Guid.NewGuid(), "Tester", "tester@dotcruz.com"))
            .Build();

        var handler = CreateHandler(schedules, authClient, notifications);

        await handler.Handle(new CheckAlertsCommand(), TestContext.Current.CancellationToken);

        Assert.Equal(1, peak);
    }

    private static MaintenanceSchedule DueSchedule(Guid tenantId, Guid? userId = null)
    {
        var vehicle = VehicleBuilder.Build(tenantId: tenantId, userId: userId, initialOdometer: 50_000);

        var schedule = MaintenanceScheduleBuilder.Build(
            tenantId: tenantId,
            userId: userId ?? vehicle.UserId,
            vehicleId: vehicle.Id,
            currentOdometer: 50_000);

        typeof(MaintenanceSchedule)
            .GetProperty(nameof(MaintenanceSchedule.Vehicle))!
            .SetValue(schedule, vehicle);

        return schedule;
    }

    private static CheckAlertsCommandHandler CreateHandler(
        IReadOnlyCollection<MaintenanceSchedule> schedules,
        DotCruz.Garage.Application.Abstractions.Services.Auth.IAuthServiceClient authClient,
        NotificationServiceBuilder notifications)
    {
        var scheduleRepository = new Mock<IMaintenanceScheduleWriteRepository>();
        scheduleRepository
            .Setup(r => r.GetSchedulesWithVehicleDetailsToUpdateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedules);

        var fineRepository = new Mock<ITrafficFineWriteRepository>();
        fineRepository
            .Setup(r => r.GetPendingFinesToNotifyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var documentRepository = new Mock<IVehicleDocumentWriteRepository>();
        documentRepository
            .Setup(r => r.GetDocumentsToNotifyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        return new CheckAlertsCommandHandler(
            scheduleRepository.Object,
            fineRepository.Object,
            documentRepository.Object,
            authClient,
            notifications.Build(),
            UnitOfWorkBuilder.Build());
    }
}
