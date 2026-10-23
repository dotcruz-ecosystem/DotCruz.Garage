using Bogus;
using DotCruz.Garage.Domain.Entities.ServiceOrders;
using DotCruz.Garage.Domain.Enums;

namespace CommonTestUtilities.Entities;

public class ServiceOrderBuilder
{
    public static ServiceOrder Build(
        Guid? workspaceId = null,
        Guid? userId = null,
        Guid? vehicleId = null,
        string? orderNumber = "OS-12345",
        string? workshopName = "Oficina Teste",
        int odometerAtService = 50_000,
        decimal totalCost = 350.00m,
        DateTimeOffset? performedAt = null,
        bool withItem = true)
    {
        var faker = new Faker();

        var serviceOrder = ServiceOrder.Create(
            workspaceId: workspaceId ?? Guid.NewGuid(),
            userId: userId ?? Guid.NewGuid(),
            vehicleId: vehicleId ?? Guid.NewGuid(),
            orderNumber: orderNumber,
            workshopName: workshopName,
            performedAt: performedAt ?? DateTimeOffset.UtcNow,
            odometerAtService: odometerAtService,
            totalCost: totalCost,
            discount: 0,
            notes: faker.Lorem.Sentence());

        if (withItem)
        {
            serviceOrder.AddItem(
                description: faker.Commerce.ProductName(),
                type: ServiceOrderItemType.Labor,
                category: MaintenanceCategory.Engine,
                quantity: 1,
                unitPrice: totalCost,
                totalCost: totalCost);
        }

        return serviceOrder;
    }
}
