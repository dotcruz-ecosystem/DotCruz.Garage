using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DotCruz.Garage.Application.UseCases.Fines.Queries.GetPendingNICFines;
using DotCruz.Garage.Domain.Enums;

namespace UseCases.Test.Fines;

public class GetPendingNICFinesQueryHandlerTest
{
    [Fact]
    public async Task Returns_Only_Fines_At_NIC_Risk_Sorted_By_Deadline()
    {
        var fineUrgent = TrafficFineBuilder.Build(
            driverId: null,
            indicationDeadline: DateTimeOffset.UtcNow.AddDays(5),
            status: FineStatus.Pending);

        var fineApproaching = TrafficFineBuilder.Build(
            driverId: null,
            indicationDeadline: DateTimeOffset.UtcNow.AddDays(12),
            status: FineStatus.Pending);

        var fineFar = TrafficFineBuilder.Build(
            driverId: null,
            indicationDeadline: DateTimeOffset.UtcNow.AddDays(30),
            status: FineStatus.Pending);

        var (fineRead, _) = new TrafficFineRepositoryBuilder()
            .SetupGetPendingNICFines(new[] { fineUrgent, fineApproaching, fineFar })
            .Build();

        var handler = new GetPendingNICFinesQueryHandler(fineRead);

        var query = new GetPendingNICFinesQuery();
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.FineId == fineUrgent.Id && r.IsUrgentRisk);
        Assert.Contains(result, r => r.FineId == fineApproaching.Id && !r.IsUrgentRisk);
    }
}
