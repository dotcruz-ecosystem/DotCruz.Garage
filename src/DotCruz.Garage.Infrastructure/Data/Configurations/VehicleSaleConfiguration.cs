using DotCruz.Garage.Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotCruz.Garage.Infrastructure.Data.Configurations;

public class VehicleSaleConfiguration : IEntityTypeConfiguration<VehicleSale>
{
    public void Configure(EntityTypeBuilder<VehicleSale> builder)
    {
        builder.ToTable("vehicle_sales");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.TenantId)
            .IsRequired();

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.VehicleId)
            .IsRequired();

        builder.Property(s => s.CustomerName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s => s.CustomerEmail)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s => s.CustomerPhone)
            .HasMaxLength(30)
            .IsRequired(false);

        builder.Property(s => s.CustomerDocument)
            .HasMaxLength(30)
            .IsRequired(false);

        builder.Property(s => s.SoldAt)
            .IsRequired();

        builder.Property(s => s.SoldPrice)
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(s => s.SoldOdometer)
            .IsRequired();

        builder.Property(s => s.WarrantyExpiresAt)
            .IsRequired(false);

        builder.Property(s => s.WarrantyKm)
            .IsRequired(false);

        builder.Property(s => s.Notes)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(s => s.ClaimedByUserId)
            .IsRequired(false);

        builder.Property(s => s.ClaimedAt)
            .IsRequired(false);

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(s => new { s.TenantId, s.VehicleId });
        builder.HasIndex(s => s.VehicleId);
        builder.HasIndex(s => s.CustomerEmail);
    }
}
