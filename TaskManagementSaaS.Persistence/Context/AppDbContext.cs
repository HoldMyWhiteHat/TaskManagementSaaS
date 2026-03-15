using Microsoft.EntityFrameworkCore;
using TaskManagementSaaS.Domain.Entities;

namespace TaskManagementSaaS.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Tenant> Tenants { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<TaskItem> Tasks { get; set; }
    }
}
// REPRESENTS MY DATABASE CONTEXT FOR MY APPLICATION
