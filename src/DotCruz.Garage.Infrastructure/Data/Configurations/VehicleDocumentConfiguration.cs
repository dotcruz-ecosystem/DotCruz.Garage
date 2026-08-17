using DotCruz.Garage.Domain.Entities.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotCruz.Garage.Infrastructure.Data.Configurations;

public class VehicleDocumentConfiguration : IEntityTypeConfiguration<VehicleDocument>
{
    public void Configure(EntityTypeBuilder<VehicleDocument> builder)
    {
        builder.ToTable("vehicle_documents");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedNever();

        builder.Property(d => d.Type)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(d => d.ReferenceYear)
            .IsRequired();

        builder.Property(d => d.DueDate)
            .IsRequired(false);

        builder.Property(d => d.Cost)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(d => d.ExpirationDate)
            .IsRequired(false);

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(d => d.FileUrl)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(d => d.LastNotifiedAt)
            .IsRequired(false);

        builder.Property(d => d.LastNotifiedStatus)
            .HasConversion<string>()
            .IsRequired(false)
            .HasMaxLength(30);

        builder.Property(d => d.WarningThresholdDays)
            .IsRequired(false);

        builder.Property(d => d.TenantId)
            .IsRequired();

        builder.Property(d => d.UserId)
            .IsRequired();

        builder.HasIndex(d => d.VehicleId);
        builder.HasIndex(d => new { d.TenantId, d.UserId });
    }
}
