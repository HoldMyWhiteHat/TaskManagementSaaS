using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSaaS.Persistence.Context;
using TaskManagementSaaS.Domain.Entities;

namespace TaskManagementSaaS.Application.Commands.Auth
{
    public record SyncUserCommand(string SubjectId, string Email, string Name, string? TenantName) : IRequest<SyncUserResult>;

    public record SyncUserResult(bool IsSuccess, string? ErrorMessage, string? Role = null, Guid? TenantId = null, string? TenantName = null, bool RequiresTenantName = false);

    public class SyncUserCommandHandler : IRequestHandler<SyncUserCommand, SyncUserResult>
    {
        private readonly AppDbContext _context;

        public SyncUserCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SyncUserResult> Handle(SyncUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Check if user already exists (by ExternalSubjectId or Email)
            var user = await _context.Users
                .IgnoreQueryFilters()
                .Include(u => u.Tenant)
                .FirstOrDefaultAsync(u => u.ExternalSubjectId == request.SubjectId || u.Email == request.Email, cancellationToken);

            // --- EXISTING USER: link Auth0 subject and return ---
            if (user != null)
            {
                bool changed = false;

                // Link Auth0 subject if not yet linked (pre-created user signing in for first time)
                if (string.IsNullOrEmpty(user.ExternalSubjectId))
                {
                    user.ExternalSubjectId = request.SubjectId;
                    changed = true;
                }

                // Update profile info if changed
                if (user.Email != request.Email || user.Username != request.Name)
                {
                    user.Email = request.Email;
                    user.Username = request.Name;
                    changed = true;
                }

                if (changed)
                {
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return new SyncUserResult(true, null, user.Role, user.TenantId, user.Tenant?.Name);
            }

            // --- NEW SIGNUP (Admin): must provide workspace name ---
            if (string.IsNullOrEmpty(request.TenantName))
            {
                return new SyncUserResult(false, null, RequiresTenantName: true);
            }

            // Check for duplicate workspace name
            var nameExists = await _context.Tenants
                .IgnoreQueryFilters()
                .AnyAsync(t => t.Name == request.TenantName, cancellationToken);

            if (nameExists)
            {
                return new SyncUserResult(false, "A workspace with this name already exists.");
            }

            // Create new workspace (tenant)
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = request.TenantName
            };
            _context.Tenants.Add(tenant);

            // Create the admin user
            user = new User
            {
                Id = Guid.NewGuid(),
                ExternalSubjectId = request.SubjectId,
                Username = request.Name,
                Email = request.Email,
                TenantId = tenant.Id,
                Role = "Admin"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            return new SyncUserResult(true, null, user.Role, user.TenantId, tenant.Name);
        }
    }
}
