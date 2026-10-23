using DotCruz.Garage.Domain.Entities.Vehicles;
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

        builder.Property(b => b.Engine)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.Type)
            .IsRequired();

        builder.Property(b => b.YearFrom)
            .IsRequired();

        builder.Property(b => b.YearTo)
            .IsRequired();

        builder.Property(b => b.SourceUrl)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(b => b.Status)
            .IsRequired();

        builder.Property(b => b.ErrorMessage)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.HasIndex(b => new { b.Brand, b.Model, b.Engine, b.Type });

        builder.HasMany(b => b.Templates)
            .WithOne(t => t.Blueprint)
            .HasForeignKey(t => t.BlueprintId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
