using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSaaS.Persistence.Context;

namespace TaskManagementSaaS.Application.Commands.Users
{
    public record DeleteUserCommand(Guid UserId) : IRequest<bool>;

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeleteUserCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null) return false;

            // Remove project associations
            var projectUsers = await _context.ProjectUsers
                .IgnoreQueryFilters()
                .Where(pu => pu.UserId == user.Id)
                .ToListAsync(cancellationToken);
            
            _context.ProjectUsers.RemoveRange(projectUsers);

            // Unassign tasks (or keep them as unassigned)
            // For now, assigned task's AssignedUserId to null or keep them
            // Maybe set them to null.
            var tasks = await _context.Tasks
                .IgnoreQueryFilters()
                .Where(t => t.AssignedUserId == user.Id)
                .ToListAsync(cancellationToken);
            
            foreach(var task in tasks)
            {
                task.AssignedUserId = null;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
