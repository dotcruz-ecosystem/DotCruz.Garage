using DotCruz.Garage.Domain.Entities.Maintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotCruz.Garage.Infrastructure.Data.Configurations;

public class MaintenanceScheduleCheckpointConfiguration : IEntityTypeConfiguration<MaintenanceScheduleCheckpoint>
{
    public void Configure(EntityTypeBuilder<MaintenanceScheduleCheckpoint> builder)
    {
        builder.ToTable("maintenance_schedule_checkpoints");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.ScheduleId).IsRequired();
        builder.Property(c => c.Odometer).IsRequired();
        builder.Property(c => c.Months).IsRequired();
        builder.Property(c => c.Order).IsRequired();
        builder.Property(c => c.IsCompleted).IsRequired();
        builder.Property(c => c.CompletedAt).IsRequired(false);
        builder.Property(c => c.CompletedOdometer).IsRequired(false);

        builder.HasIndex(c => c.ScheduleId);
    }
}
