using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSaaS.Persistence.Context;

namespace TaskManagementSaaS.Application.Commands.Projects
{
    public record DeleteProjectCommand(Guid ProjectId) : IRequest<bool>;

    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeleteProjectCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

            if (project == null) return false;

            // Tasks will be deleted automatically if cascade is on.
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
