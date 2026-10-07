using DotCruz.Garage.Domain.Entities.Drivers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotCruz.Garage.Infrastructure.Data.Configurations;

public class VehicleDriverAssignmentConfiguration : IEntityTypeConfiguration<VehicleDriverAssignment>
{
    public void Configure(EntityTypeBuilder<VehicleDriverAssignment> builder)
    {
        builder.ToTable("vehicle_driver_assignments");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.VehicleId)
            .IsRequired();

        builder.Property(a => a.DriverId)
            .IsRequired();

        builder.Property(a => a.StartedAt)
            .IsRequired();

        builder.Property(a => a.EndedAt)
            .IsRequired(false);

        builder.Property(a => a.Notes)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(a => a.WorkspaceId)
            .IsRequired();

        builder.Property(a => a.UserId)
            .IsRequired();

        builder.HasOne(a => a.Vehicle)
            .WithMany()
            .HasForeignKey(a => a.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Driver)
            .WithMany(d => d.Assignments)
            .HasForeignKey(a => a.DriverId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.VehicleId);
        builder.HasIndex(a => a.DriverId);
        builder.HasIndex(a => new { a.WorkspaceId, a.VehicleId, a.EndedAt });
    }
}
