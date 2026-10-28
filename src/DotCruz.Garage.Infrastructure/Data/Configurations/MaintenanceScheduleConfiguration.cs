using DotCruz.Garage.Domain.Entities.Maintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotCruz.Garage.Infrastructure.Data.Configurations;

public class MaintenanceScheduleConfiguration : IEntityTypeConfiguration<MaintenanceSchedule>
{
    public void Configure(EntityTypeBuilder<MaintenanceSchedule> builder)
    {
        builder.ToTable("maintenance_schedules");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Category)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(s => s.IntervalOdometer)
            .IsRequired(false);

        builder.Property(s => s.IntervalTimeMonths)
            .IsRequired(false);

        builder.Property(s => s.LastPerformedOdometer)
            .IsRequired(false);

        builder.Property(s => s.LastPerformedDate)
            .IsRequired(false);

        builder.Property(s => s.NextDueOdometer)
            .IsRequired(false);

        builder.Property(s => s.NextDueDate)
            .IsRequired(false);

        builder.Property(s => s.LastNotifiedAt)
            .IsRequired(false);

        builder.Property(s => s.LastNotifiedStatus)
            .HasConversion<string>()
            .IsRequired(false)
            .HasMaxLength(30);

        builder.Property(s => s.WarningThresholdOdometer)
            .IsRequired(false);

        builder.Property(s => s.WarningThresholdDays)
            .IsRequired(false);

        builder.Property(s => s.OriginWorkspaceId)
            .IsRequired(false);

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.HasMany(s => s.Checkpoints)
            .WithOne(c => c.Schedule)
            .HasForeignKey(c => c.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.VehicleId);
        builder.HasIndex(s => s.OriginWorkspaceId);
        builder.HasIndex(s => s.UserId);
    }
}
