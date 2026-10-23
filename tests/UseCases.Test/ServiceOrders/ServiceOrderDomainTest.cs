using DotCruz.Garage.Domain.Entities.ServiceOrders;
using DotCruz.Garage.Domain.Enums;

namespace UseCases.Test.ServiceOrders;

public class ServiceOrderDomainTest
{
    [Fact]
    public void Creates_ServiceOrder_With_Items_And_Attachments_Successfully()
    {
        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var performedAt = DateTimeOffset.UtcNow;

        var so = ServiceOrder.Create(
            workspaceId,
            userId,
            vehicleId,
            "39196",
            "Moto Selvagem",
            performedAt,
            24_891,
            178.70m,
            0,
            "Manutenção periódica");

        var item1 = so.AddItem(
            "Troca Vela Ignição",
            ServiceOrderItemType.Labor,
            MaintenanceCategory.Engine,
            1,
            100m,
            100m,
            Guid.NewGuid());

        var item2 = so.AddItem(
            "Parafuso Pedal Câmbio",
            ServiceOrderItemType.Part,
            MaintenanceCategory.General,
            1,
            13.70m,
            13.70m);

        var attachment = so.AddAttachment("nota.jpg", "https://storage.dotcruz.com/nota.jpg", "image/jpeg", 2048);

        Assert.Equal("39196", so.OrderNumber);
        Assert.Equal("Moto Selvagem", so.WorkshopName);
        Assert.Equal(24_891, so.OdometerAtService);
        Assert.Equal(178.70m, so.TotalCost);
        Assert.Equal(2, so.Items.Count);
        Assert.Single(so.Attachments);

        so.RemoveAttachment(attachment.Id);
        Assert.Empty(so.Attachments);
    }
}
