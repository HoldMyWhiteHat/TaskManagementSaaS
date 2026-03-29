using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSaaS.Persistence.Context;

namespace TaskManagementSaaS.Application.Commands.Users
{
    public record DeleteMyAccountCommand(string SubjectId) : IRequest<bool>;

    public class DeleteMyAccountCommandHandler : IRequestHandler<DeleteMyAccountCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeleteMyAccountCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteMyAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .IgnoreQueryFilters()
                .Include(u => u.Tenant)
                .FirstOrDefaultAsync(u => u.ExternalSubjectId == request.SubjectId, cancellationToken);

            if (user == null) return false;

            if (user.Role == "Admin" && user.TenantId != Guid.Empty)
            {
                var tenantId = user.TenantId;

                // Manually remove related entities to prevent restrictive FK cascade errors (DbUpdateException)
                var projectUsers = await _context.ProjectUsers
                    .IgnoreQueryFilters()
                    .Where(pu => pu.TenantId == tenantId)
                    .ToListAsync(cancellationToken);
                _context.ProjectUsers.RemoveRange(projectUsers);

                var activityLogs = await _context.ActivityLogs
                    .IgnoreQueryFilters()
                    .Where(al => al.TenantId == tenantId)
                    .ToListAsync(cancellationToken);
                _context.ActivityLogs.RemoveRange(activityLogs);

                var tasks = await _context.Tasks
                    .IgnoreQueryFilters()
                    .Where(t => t.TenantId == tenantId)
                    .ToListAsync(cancellationToken);
                _context.Tasks.RemoveRange(tasks);

                var projects = await _context.Projects
                    .IgnoreQueryFilters()
                    .Where(p => p.TenantId == tenantId)
                    .ToListAsync(cancellationToken);
                _context.Projects.RemoveRange(projects);

                var allTenantUsers = await _context.Users
                    .IgnoreQueryFilters()
                    .Where(u => u.TenantId == tenantId)
                    .ToListAsync(cancellationToken);
                _context.Users.RemoveRange(allTenantUsers);

                var tenant = await _context.Tenants
                    .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);
                
                if (tenant != null)
                {
                    _context.Tenants.Remove(tenant);
                }
            }
            else
            {
                // Simple user deletion
                var projectUsers = await _context.ProjectUsers
                    .IgnoreQueryFilters()
                    .Where(pu => pu.UserId == user.Id)
                    .ToListAsync(cancellationToken);
                _context.ProjectUsers.RemoveRange(projectUsers);

                var tasks = await _context.Tasks
                    .IgnoreQueryFilters()
                    .Where(t => t.AssignedUserId == user.Id)
                    .ToListAsync(cancellationToken);
                
                foreach(var task in tasks)
                {
                    task.AssignedUserId = null;
                }

                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
