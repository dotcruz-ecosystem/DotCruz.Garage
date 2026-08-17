using DotCruz.Garage.Domain.Entities.Maintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotCruz.Garage.Infrastructure.Data.Configurations;

public class MaintenanceLogConfiguration : IEntityTypeConfiguration<MaintenanceLog>
{
    public void Configure(EntityTypeBuilder<MaintenanceLog> builder)
    {
        builder.ToTable("maintenance_logs");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedNever();

        builder.Property(l => l.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(l => l.PerformedAt)
            .IsRequired();

        builder.Property(l => l.OdometerAtService)
            .IsRequired();

        builder.Property(l => l.Cost)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(l => l.ReceiptUrl)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(l => l.Notes)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(l => l.TenantId)
            .IsRequired();

        builder.Property(l => l.UserId)
            .IsRequired();

        builder.HasIndex(l => l.VehicleId);
        builder.HasIndex(l => new { l.TenantId, l.UserId });
    }
}
