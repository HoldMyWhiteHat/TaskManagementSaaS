using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSaaS.Domain.Interfaces;
using TaskManagementSaaS.Persistence.Context;

namespace TaskManagementSaaS.Application.Commands.Tenants
{
    public record UpdateWorkspaceCommand(string Name) : IRequest<UpdateWorkspaceResult>;

    public record UpdateWorkspaceResult(bool IsSuccess, string? ErrorMessage);

    public class UpdateWorkspaceCommandHandler : IRequestHandler<UpdateWorkspaceCommand, UpdateWorkspaceResult>
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;

        public UpdateWorkspaceCommandHandler(AppDbContext context, IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<UpdateWorkspaceResult> Handle(UpdateWorkspaceCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _userContext.GetTenantId();
            if (tenantId == null) return new UpdateWorkspaceResult(false, "No tenant context found.");

            var tenant = await _context.Tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == tenantId.Value, cancellationToken);

            if (tenant == null) return new UpdateWorkspaceResult(false, "Workspace not found.");

            // Check for duplicate name (excluding current tenant)
            if (tenant.Name != request.Name)
            {
                var nameExists = await _context.Tenants
                    .IgnoreQueryFilters()
                    .AnyAsync(t => t.Name == request.Name && t.Id != tenantId.Value, cancellationToken);

                if (nameExists)
                {
                    return new UpdateWorkspaceResult(false, "A workspace with this name already exists.");
                }
            }

            tenant.Name = request.Name;
            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateWorkspaceResult(true, null);
        }
    }
}
