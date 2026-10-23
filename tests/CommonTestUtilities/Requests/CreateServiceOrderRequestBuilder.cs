using Bogus;
using DotCruz.Garage.Application.DTOs.ServiceOrders;
using DotCruz.Garage.Domain.Enums;

namespace CommonTestUtilities.Requests;

public class CreateServiceOrderRequestBuilder
{
    public static CreateServiceOrderRequestDto Build(
        int odometerAtService = 50_000,
        decimal totalCost = 150m,
        Guid? scheduleId = null,
        DateTimeOffset? performedAt = null)
    {
        var faker = new Faker();

        return new CreateServiceOrderRequestDto(
            OrderNumber: "OS-" + faker.Random.Number(1000, 9999),
            WorkshopName: faker.Company.CompanyName(),
            PerformedAt: performedAt ?? DateTimeOffset.UtcNow,
            OdometerAtService: odometerAtService,
            TotalCost: totalCost,
            Discount: 0,
            Notes: faker.Lorem.Sentence(),
            Items: new List<CreateServiceOrderItemRequestDto>
            {
                new(
                    Description: "Troca de Vela",
                    Type: ServiceOrderItemType.Labor,
                    Category: MaintenanceCategory.Engine,
                    Quantity: 1,
                    UnitPrice: 100m,
                    TotalCost: 100m,
                    MaintenanceScheduleId: scheduleId),
                new(
                    Description: "Parafuso",
                    Type: ServiceOrderItemType.Part,
                    Category: MaintenanceCategory.General,
                    Quantity: 1,
                    UnitPrice: 50m,
                    TotalCost: 50m,
                    MaintenanceScheduleId: null)
            });
    }
}
