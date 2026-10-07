using DotCruz.Garage.Domain.Entities.Drivers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotCruz.Garage.Infrastructure.Data.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.ToTable("drivers");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedNever();

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(d => d.Cpf)
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(d => d.Cnh)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(d => d.CnhCategory)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(d => d.CnhExpirationDate)
            .IsRequired();

        builder.Property(d => d.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(d => d.Phone)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(d => d.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(d => d.WorkspaceId)
            .IsRequired();

        builder.Property(d => d.UserId)
            .IsRequired();

        builder.HasIndex(d => new { d.WorkspaceId, d.Cpf }).IsUnique();
        builder.HasIndex(d => new { d.WorkspaceId, d.Cnh });
        builder.HasIndex(d => new { d.WorkspaceId, d.UserId });
    }
}
