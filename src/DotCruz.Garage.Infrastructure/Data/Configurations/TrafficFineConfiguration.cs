using DotCruz.Garage.Domain.Entities.Fines;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotCruz.Garage.Infrastructure.Data.Configurations;

public class TrafficFineConfiguration : IEntityTypeConfiguration<TrafficFine>
{
    public void Configure(EntityTypeBuilder<TrafficFine> builder)
    {
        builder.ToTable("traffic_fines");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).ValueGeneratedNever();

        builder.Property(f => f.InfractionCode)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(f => f.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(f => f.InfractionDate)
            .IsRequired();

        builder.Property(f => f.NotificationDate)
            .IsRequired(false);

        builder.Property(f => f.DueDate)
            .IsRequired();

        builder.Property(f => f.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(f => f.DiscountAmount)
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(f => f.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(f => f.ReceiptUrl)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(f => f.LastNotifiedAt)
            .IsRequired(false);

        builder.Property(f => f.WarningThresholdDays)
            .IsRequired(false);

        builder.Property(f => f.DriverId)
            .IsRequired(false);

        builder.Property(f => f.IssuingAuthority)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(f => f.Points)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(f => f.IndicationDeadline)
            .IsRequired(false);

        builder.Property(f => f.IndicatedAt)
            .IsRequired(false);

        builder.Property(f => f.IndicationProtocol)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(f => f.TenantId)
            .IsRequired();

        builder.Property(f => f.UserId)
            .IsRequired();

        builder.HasOne(f => f.Driver)
            .WithMany(d => d.Fines)
            .HasForeignKey(f => f.DriverId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(f => f.VehicleId);
        builder.HasIndex(f => f.DriverId);
        builder.HasIndex(f => f.IndicationDeadline);
        builder.HasIndex(f => new { f.TenantId, f.UserId });
    }
}
