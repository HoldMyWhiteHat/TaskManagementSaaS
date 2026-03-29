using Microsoft.EntityFrameworkCore;
using TaskManagementSaaS.Domain.Entities;
using TaskManagementSaaS.Domain.Interfaces;

namespace TaskManagementSaaS.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        private readonly IUserContextService? _userContext;

        // EF Core query filters need to reference a property on the DbContext
        // so the value is re-evaluated per query (not baked in at model creation time).
        private Guid? TenantId => _userContext?.GetTenantId();

        public AppDbContext(DbContextOptions<AppDbContext> options, IUserContextService? userContext = null)
            : base(options)
        {
            _userContext = userContext;
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Global Query Filters for Tenant Isolation
            // Reference TenantId property so EF re-evaluates per query
            modelBuilder.Entity<Project>().HasQueryFilter(p => p.TenantId == TenantId);
            modelBuilder.Entity<TaskItem>().HasQueryFilter(t => t.TenantId == TenantId);
            modelBuilder.Entity<User>().HasQueryFilter(u => u.TenantId == TenantId);
            modelBuilder.Entity<ProjectUser>().HasQueryFilter(pu => pu.TenantId == TenantId);
            modelBuilder.Entity<ActivityLog>().HasQueryFilter(al => al.TenantId == TenantId);

            // Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}

