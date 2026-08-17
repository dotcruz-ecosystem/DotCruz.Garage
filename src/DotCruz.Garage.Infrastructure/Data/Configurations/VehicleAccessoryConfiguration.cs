using DotCruz.Garage.Domain.Entities.Accessories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotCruz.Garage.Infrastructure.Data.Configurations;

public class VehicleAccessoryConfiguration : IEntityTypeConfiguration<VehicleAccessory>
{
    public void Configure(EntityTypeBuilder<VehicleAccessory> builder)
    {
        builder.ToTable("vehicle_accessories");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Store)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(a => a.PurchaseDate)
            .IsRequired();

        builder.Property(a => a.Cost)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(a => a.ReceiptUrl)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(a => a.Notes)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(a => a.TenantId)
            .IsRequired();

        builder.Property(a => a.UserId)
            .IsRequired();

        builder.HasIndex(a => a.VehicleId);
        builder.HasIndex(a => new { a.TenantId, a.UserId });
    }
}
