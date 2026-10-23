using DotCruz.Garage.Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotCruz.Garage.Infrastructure.Data.Configurations;

public class VehicleEngineConfiguration : IEntityTypeConfiguration<VehicleEngine>
{
    public void Configure(EntityTypeBuilder<VehicleEngine> builder)
    {
        builder.ToTable("vehicle_engines");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Type)
            .IsRequired(false);

        builder.HasIndex(e => new { e.Name, e.Type })
            .IsUnique();
    }
}
