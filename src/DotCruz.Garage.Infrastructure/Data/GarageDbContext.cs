using System.Linq.Expressions;
using DotCruz.Shared.Security.Context;
using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Entities.Maintenance;
using DotCruz.Garage.Domain.Entities.Fines;
using DotCruz.Garage.Domain.Entities.Documents;
using DotCruz.Garage.Domain.Entities.Accessories;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data;

public class GarageDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;

    public GarageDbContext(DbContextOptions<GarageDbContext> options, ITenantProvider tenantProvider) 
        : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<OdometerHistory> OdometerHistories { get; set; }
    public DbSet<MaintenanceSchedule> MaintenanceSchedules { get; set; }
    public DbSet<MaintenanceLog> MaintenanceLogs { get; set; }
    public DbSet<TrafficFine> TrafficFines { get; set; }
    public DbSet<VehicleDocument> VehicleDocuments { get; set; }
    public DbSet<VehicleAccessory> VehicleAccessories { get; set; }

    public Guid CurrentTenantId => _tenantProvider.TenantId ?? Guid.Empty;
    public Guid CurrentUserId => _tenantProvider.UserId ?? Guid.Empty;
    public bool ShouldFilterByUserId => _tenantProvider.FilterByUserId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GarageDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(e => e.ClrType != null))
        {
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            Expression? combinedExpr = null;

            if (typeof(ISoftDeleteEntity).IsAssignableFrom(entityType.ClrType))
            {
                var propertyDeletedAt = Expression.Property(parameter, nameof(ISoftDeleteEntity.DeletedAt));
                var nullValue = Expression.Constant(null, typeof(DateTimeOffset?));
                var softDeleteExpr = Expression.Equal(propertyDeletedAt, nullValue);
                combinedExpr = softDeleteExpr;
            }

            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var propertyTenantId = Expression.Property(parameter, nameof(ITenantEntity.TenantId));
                var currentTenantIdProp = Expression.Property(Expression.Constant(this), nameof(CurrentTenantId));
                var tenantExpr = Expression.Equal(propertyTenantId, currentTenantIdProp);

                var propertyUserId = Expression.Property(parameter, nameof(ITenantEntity.UserId));
                var currentUserIdProp = Expression.Property(Expression.Constant(this), nameof(CurrentUserId));
                var userExpr = Expression.Equal(propertyUserId, currentUserIdProp);

                var shouldFilterByUserIdProp = Expression.Property(Expression.Constant(this), nameof(ShouldFilterByUserId));
                var notShouldFilterByUserId = Expression.Not(shouldFilterByUserIdProp);

                var userFilterExpr = Expression.OrElse(notShouldFilterByUserId, userExpr);

                var tenantAndUserExpr = Expression.AndAlso(tenantExpr, userFilterExpr);

                combinedExpr = combinedExpr != null 
                    ? Expression.AndAlso(combinedExpr, tenantAndUserExpr) 
                    : tenantAndUserExpr;
            }

            if (combinedExpr != null)
            {
                var lambda = Expression.Lambda(combinedExpr, parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is BaseEntity baseEntity && entry.State == EntityState.Modified)
            {
                baseEntity.Touch();
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
