using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSaaS.Persistence.Context;
using TaskManagementSaaS.Domain.Enums;
using TaskManagementSaaS.Domain.Interfaces;
using TaskManagementSaaS.Domain.Entities;

namespace TaskManagementSaaS.Application.Commands.Tasks
{
    public record CompleteTaskCommand(Guid TaskId) : IRequest<bool>;

    public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand, bool>
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;

        public CompleteTaskCommandHandler(AppDbContext context, IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<bool> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

            if (task == null) return false;

            // Only the assigned user can complete the task
            var userId = _userContext.GetUserId();
            if (!userId.HasValue || task.AssignedUserId != userId.Value) return false;

            task.Status = TaskItemStatus.Completed;

            _context.ActivityLogs.Add(new ActivityLog
            {
                Id = Guid.NewGuid(),
                Message = $"Task '{task.Title}' was completed.",
                UserEmail = _userContext.GetUserEmail() ?? "Unknown",
                Timestamp = DateTime.UtcNow,
                TenantId = task.TenantId
            });

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
