using DotCruz.Garage.Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotCruz.Garage.Infrastructure.Data.Configurations;

public class OdometerHistoryConfiguration : IEntityTypeConfiguration<OdometerHistory>
{
    public void Configure(EntityTypeBuilder<OdometerHistory> builder)
    {
        builder.ToTable("odometer_histories");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();

        builder.Property(o => o.OdometerValue)
            .IsRequired();

        builder.Property(o => o.RecordedAt)
            .IsRequired();

        builder.HasIndex(o => o.VehicleId);
    }
}
