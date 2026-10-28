using DotCruz.Garage.Domain.Entities.ServiceOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotCruz.Garage.Infrastructure.Data.Configurations;

public class ServiceOrderConfiguration : IEntityTypeConfiguration<ServiceOrder>
{
    public void Configure(EntityTypeBuilder<ServiceOrder> builder)
    {
        builder.ToTable("service_orders");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();

        builder.Property(o => o.VehicleId).IsRequired();
        builder.Property(o => o.OriginWorkspaceId).IsRequired(false);
        builder.Property(o => o.UserId).IsRequired();

        builder.Property(o => o.OrderNumber)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(o => o.WorkshopName)
            .HasMaxLength(150)
            .IsRequired(false);

        builder.Property(o => o.PerformedAt)
            .IsRequired();

        builder.Property(o => o.OdometerAtService)
            .IsRequired();

        builder.Property(o => o.TotalCost)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(o => o.Discount)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(o => o.Notes)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.HasIndex(o => o.VehicleId);
        builder.HasIndex(o => o.OriginWorkspaceId);
        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => new { o.VehicleId, o.PerformedAt });

        builder.HasOne(o => o.Vehicle)
            .WithMany()
            .HasForeignKey(o => o.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
            .WithOne(i => i.ServiceOrder)
            .HasForeignKey(i => i.ServiceOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Attachments)
            .WithOne(a => a.ServiceOrder)
            .HasForeignKey(a => a.ServiceOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
