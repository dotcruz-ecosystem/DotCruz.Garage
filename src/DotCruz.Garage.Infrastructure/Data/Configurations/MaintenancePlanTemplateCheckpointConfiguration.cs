using DotCruz.Garage.Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotCruz.Garage.Infrastructure.Data.Configurations;

public class MaintenancePlanTemplateCheckpointConfiguration : IEntityTypeConfiguration<MaintenancePlanTemplateCheckpoint>
{
    public void Configure(EntityTypeBuilder<MaintenancePlanTemplateCheckpoint> builder)
    {
        builder.ToTable("maintenance_plan_template_checkpoints");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.TemplateId).IsRequired();
        builder.Property(c => c.Odometer).IsRequired();
        builder.Property(c => c.Months).IsRequired();
        builder.Property(c => c.Order).IsRequired();

        builder.HasIndex(c => c.TemplateId);
    }
}
