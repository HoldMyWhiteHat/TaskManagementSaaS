using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSaaS.Application.DTO.Projects;
using TaskManagementSaaS.Persistence.Context;

namespace TaskManagementSaaS.Application.Queries.Projects
{
    public record GetAllProjectsQuery() : IRequest<List<ProjectDto>>;

    public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, List<ProjectDto>>
    {
        private readonly AppDbContext _context;

        public GetAllProjectsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectDto>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
        {
            // All users see all projects within their workspace.
            // Tenant isolation is handled by the global query filter in AppDbContext.
            var query = _context.Projects.AsQueryable();

            return await query
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    CreatedAt = p.CreatedAt,
                    TaskCount = p.Tasks.Count(),
                    MemberCount = p.ProjectUsers.Count(),
                    Members = p.ProjectUsers.Select(pu => new ProjectMemberDto
                    {
                        Id = pu.UserId,
                        Username = pu.User != null ? pu.User.Username : "Unknown"
                    }).ToList()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
