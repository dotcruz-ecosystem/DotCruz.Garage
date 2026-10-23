using DotCruz.Garage.Domain.Entities.Vehicles;
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

        builder.Property(t => t.BlueprintId)
            .IsRequired();

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Category)
            .IsRequired();

        builder.Property(t => t.IntervalOdometer)
            .IsRequired(false);

        builder.Property(t => t.IntervalTimeMonths)
            .IsRequired(false);

        builder.Property(t => t.IsRequired)
            .IsRequired();

        builder.HasIndex(t => t.BlueprintId);
    }
}
