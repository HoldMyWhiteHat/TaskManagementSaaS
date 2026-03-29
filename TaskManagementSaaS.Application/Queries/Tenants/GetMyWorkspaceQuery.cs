using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSaaS.Application.DTO.Tenants;
using TaskManagementSaaS.Domain.Interfaces;
using TaskManagementSaaS.Persistence.Context;

namespace TaskManagementSaaS.Application.Queries.Tenants
{
    public record GetMyWorkspaceQuery() : IRequest<TenantDto?>;

    public class GetMyWorkspaceQueryHandler : IRequestHandler<GetMyWorkspaceQuery, TenantDto?>
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;

        public GetMyWorkspaceQueryHandler(AppDbContext context, IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<TenantDto?> Handle(GetMyWorkspaceQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _userContext.GetTenantId();
            if (tenantId == null) return null;

            var tenant = await _context.Tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == tenantId.Value, cancellationToken);

            if (tenant == null) return null;

            return new TenantDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                CreatedAt = tenant.CreatedAt
            };
        }
    }
}
