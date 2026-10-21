using System.Linq.Expressions;
using DotCruz.Shared.Security.Context;
using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Entities.Vehicles;
using DotCruz.Garage.Domain.Entities.Maintenance;
using Microsoft.EntityFrameworkCore;

namespace DotCruz.Garage.Infrastructure.Data;

public class GarageDbContext : DbContext
{
    private readonly IWorkspaceProvider _workspaceProvider;

    public GarageDbContext(DbContextOptions<GarageDbContext> options, IWorkspaceProvider workspaceProvider) 
        : base(options)
    {
        _workspaceProvider = workspaceProvider;
    }

    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<OdometerHistory> OdometerHistories { get; set; }
    public DbSet<MaintenanceSchedule> MaintenanceSchedules { get; set; }
    public DbSet<MaintenanceLog> MaintenanceLogs { get; set; }

    public Guid CurrentWorkspaceId => _workspaceProvider.WorkspaceId ?? Guid.Empty;
    public Guid CurrentUserId => _workspaceProvider.UserId ?? Guid.Empty;
    public bool ShouldFilterByUserId => _workspaceProvider.FilterByUserId;

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

            if (typeof(IWorkspaceEntity).IsAssignableFrom(entityType.ClrType))
            {
                var propertyWorkspaceId = Expression.Property(parameter, nameof(IWorkspaceEntity.WorkspaceId));
                var currentWorkspaceIdProp = Expression.Property(Expression.Constant(this), nameof(CurrentWorkspaceId));
                var workspaceExpr = Expression.Equal(propertyWorkspaceId, currentWorkspaceIdProp);

                var propertyUserId = Expression.Property(parameter, nameof(IWorkspaceEntity.UserId));
                var currentUserIdProp = Expression.Property(Expression.Constant(this), nameof(CurrentUserId));
                var userExpr = Expression.Equal(propertyUserId, currentUserIdProp);

                var shouldFilterByUserIdProp = Expression.Property(Expression.Constant(this), nameof(ShouldFilterByUserId));
                var notShouldFilterByUserId = Expression.Not(shouldFilterByUserIdProp);

                var userFilterExpr = Expression.OrElse(notShouldFilterByUserId, userExpr);

                var workspaceAndUserExpr = Expression.AndAlso(workspaceExpr, userFilterExpr);

                combinedExpr = combinedExpr != null 
                    ? Expression.AndAlso(combinedExpr, workspaceAndUserExpr) 
                    : workspaceAndUserExpr;
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
