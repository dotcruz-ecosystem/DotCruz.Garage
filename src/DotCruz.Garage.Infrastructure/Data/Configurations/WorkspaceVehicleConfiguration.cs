using DotCruz.Garage.Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotCruz.Garage.Infrastructure.Data.Configurations;

public class WorkspaceVehicleConfiguration : IEntityTypeConfiguration<WorkspaceVehicle>
{
    public void Configure(EntityTypeBuilder<WorkspaceVehicle> builder)
    {
        builder.ToTable("workspace_vehicles");

        builder.HasKey(wv => wv.Id);
        builder.Property(wv => wv.Id).ValueGeneratedNever();

        builder.Property(wv => wv.WorkspaceId)
            .IsRequired();

        builder.Property(wv => wv.VehicleId)
            .IsRequired();

        builder.Property(wv => wv.Role)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(wv => wv.CreatedByUserId)
            .IsRequired();

        builder.HasIndex(wv => new { wv.WorkspaceId, wv.VehicleId })
            .IsUnique();

        builder.HasIndex(wv => wv.WorkspaceId);
        builder.HasIndex(wv => wv.VehicleId);

        builder.HasOne(wv => wv.Vehicle)
            .WithMany(v => v.Workspaces)
            .HasForeignKey(wv => wv.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
