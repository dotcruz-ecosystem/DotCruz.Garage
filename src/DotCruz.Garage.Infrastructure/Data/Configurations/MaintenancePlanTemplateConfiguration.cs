using System.Text.Json;
using DotCruz.Garage.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotCruz.Garage.Infrastructure.Data.Configurations;

public class MaintenancePlanTemplateConfiguration : IEntityTypeConfiguration<MaintenancePlanTemplate>
{
    public void Configure(EntityTypeBuilder<MaintenancePlanTemplate> builder)
    {
        builder.ToTable("maintenance_plan_templates");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();

        builder.Property(t => t.VehicleBlueprintId)
            .IsRequired();

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Category)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(t => t.IntervalOdometer)
            .IsRequired(false);

        builder.Property(t => t.IntervalTimeMonths)
            .IsRequired(false);

        builder.Property(t => t.IsRecurring)
            .IsRequired();

        var stringListComparer = new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
            (c1, c2) => c1 != null && c2 != null ? c1.SequenceEqual(c2) : c1 == c2,
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        builder.Property(t => t.Items)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>(),
                stringListComparer)
            .HasColumnType("jsonb");

        builder.HasIndex(t => t.VehicleBlueprintId);
    }
}
