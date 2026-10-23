using DotCruz.Garage.Domain.Entities.ServiceOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotCruz.Garage.Infrastructure.Data.Configurations;

public class ServiceOrderItemConfiguration : IEntityTypeConfiguration<ServiceOrderItem>
{
    public void Configure(EntityTypeBuilder<ServiceOrderItem> builder)
    {
        builder.ToTable("service_order_items");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();

        builder.Property(i => i.ServiceOrderId).IsRequired();

        builder.Property(i => i.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Type)
            .IsRequired();

        builder.Property(i => i.Category)
            .IsRequired();

        builder.Property(i => i.PartNumber)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(i => i.Quantity)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.TotalCost)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.Notes)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.HasIndex(i => i.ServiceOrderId);
        builder.HasIndex(i => i.MaintenanceScheduleId);

        builder.HasOne(i => i.MaintenanceSchedule)
            .WithMany()
            .HasForeignKey(i => i.MaintenanceScheduleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
