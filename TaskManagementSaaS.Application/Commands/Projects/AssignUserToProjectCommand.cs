using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSaaS.Persistence.Context;
using TaskManagementSaaS.Domain.Entities;

namespace TaskManagementSaaS.Application.Commands.Projects
{
    public record AssignUserToProjectCommand(Guid ProjectId, Guid UserId) : IRequest<bool>;

    public class AssignUserToProjectCommandHandler : IRequestHandler<AssignUserToProjectCommand, bool>
    {
        private readonly AppDbContext _context;

        public AssignUserToProjectCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(AssignUserToProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects.FindAsync(new object[] { request.ProjectId }, cancellationToken);
            if (project == null) return false;

            var existing = await _context.ProjectUsers
                .FirstOrDefaultAsync(pu => pu.ProjectId == request.ProjectId && pu.UserId == request.UserId, cancellationToken);
            
            if (existing != null) return true;

            var projectUser = new ProjectUser
            {
                ProjectId = request.ProjectId,
                UserId = request.UserId,
                TenantId = project.TenantId
            };

            _context.ProjectUsers.Add(projectUser);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
