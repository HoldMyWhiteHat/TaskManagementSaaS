using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSaaS.Persistence.Context;
using TaskManagementSaaS.Domain.Enums;
using TaskManagementSaaS.Domain.Interfaces;

namespace TaskManagementSaaS.Application.Commands.Tasks
{
    public record DeleteTaskCommand(Guid TaskId) : IRequest<bool>;

    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;

        public DeleteTaskCommandHandler(AppDbContext context, IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _context.Tasks
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

            if (task == null) return false;

            // Role check: Only Admin can delete
            var role = _userContext.GetRole();
            if (role != "Admin") return false;

            // Status check: Only Completed tasks can be deleted
            if (task.Status != TaskItemStatus.Completed) return false;

            _context.Tasks.Remove(task);

            // Also remove associated activity logs for this task? 
            // Better to keep logs but remove the task reference if it's there.
            // Activity logs don't have a TaskId FK usually, just text.

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
