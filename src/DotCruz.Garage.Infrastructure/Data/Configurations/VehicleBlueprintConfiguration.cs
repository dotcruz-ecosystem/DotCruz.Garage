using DotCruz.Garage.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotCruz.Garage.Infrastructure.Data.Configurations;

public class VehicleBlueprintConfiguration : IEntityTypeConfiguration<VehicleBlueprint>
{
    public void Configure(EntityTypeBuilder<VehicleBlueprint> builder)
    {
        builder.ToTable("vehicle_blueprints");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever();

        builder.Property(b => b.Brand)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Year)
            .IsRequired();

        builder.Property(b => b.Engine)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(b => b.Version)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(b => b.Source)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder.HasIndex(b => new { b.Brand, b.Model, b.Year, b.Engine });

        builder.HasMany(b => b.PlanTemplates)
            .WithOne(t => t.VehicleBlueprint)
            .HasForeignKey(t => t.VehicleBlueprintId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
