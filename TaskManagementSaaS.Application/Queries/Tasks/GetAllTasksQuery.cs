using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSaaS.Application.DTO.Tasks;
using TaskManagementSaaS.Persistence.Context;

namespace TaskManagementSaaS.Application.Queries.Tasks
{
    public record GetAllTasksQuery(Guid? ProjectId = null) : IRequest<List<TaskDto>>;

    public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, List<TaskDto>>
    {
        private readonly AppDbContext _context;

        public GetAllTasksQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskDto>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Tasks.AsQueryable();

            if (request.ProjectId.HasValue)
            {
                query = query.Where(t => t.ProjectId == request.ProjectId.Value);
            }

            return await query
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Priority = t.Priority,
                    Status = t.Status,
                    ProjectId = t.ProjectId,
                    AssignedUserEmail = t.AssignedUser != null ? t.AssignedUser.Email : null,
                    AssigneeName = t.AssignedUser != null ? t.AssignedUser.Username : "Unassigned",
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}
