using Bogus;
using DotCruz.Garage.Domain.Entities.Fines;
using DotCruz.Garage.Domain.Enums;

namespace CommonTestUtilities.Entities;

public class TrafficFineBuilder
{
    public static TrafficFine Build(
        Guid? workspaceId = null,
        Guid? userId = null,
        Guid? vehicleId = null,
        Guid? driverId = null,
        string? infractionCode = "745-5-0",
        string? description = "Transitar em velocidade superior à máxima permitida em até 20%",
        int points = 4,
        decimal amount = 130.16m,
        DateTimeOffset? infractionDate = null,
        DateTimeOffset? dueDate = null,
        DateTimeOffset? indicationDeadline = null,
        FineStatus status = FineStatus.Pending)
    {
        var faker = new Faker();

        return TrafficFine.Create(
            workspaceId: workspaceId ?? Guid.NewGuid(),
            userId: userId ?? Guid.NewGuid(),
            vehicleId: vehicleId ?? Guid.NewGuid(),
            infractionCode: infractionCode,
            description: description ?? faker.Lorem.Sentence(4),
            infractionDate: infractionDate ?? DateTimeOffset.UtcNow.AddDays(-10),
            notificationDate: DateTimeOffset.UtcNow.AddDays(-5),
            dueDate: dueDate ?? DateTimeOffset.UtcNow.AddDays(30),
            amount: amount,
            discountAmount: amount * 0.8m,
            status: status,
            receiptUrl: null,
            warningThresholdDays: 10,
            driverId: driverId,
            issuingAuthority: "DETRAN-SP",
            points: points,
            indicationDeadline: indicationDeadline
        );
    }
}
