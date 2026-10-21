using DotCruz.Garage.Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotCruz.Garage.Infrastructure.Data.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("vehicles");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).ValueGeneratedNever();

        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Brand)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Year)
            .IsRequired();

        builder.Property(v => v.Plate)
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(v => v.CurrentOdometer)
            .IsRequired();

        builder.Property(v => v.DailyMileageAverage)
            .IsRequired();

        builder.Property(v => v.WorkspaceId)
            .IsRequired();

        builder.Property(v => v.UserId)
            .IsRequired();

        builder.HasIndex(v => new { v.WorkspaceId, v.UserId });

        builder.HasMany(v => v.OdometerHistories)
            .WithOne()
            .HasForeignKey(o => o.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
