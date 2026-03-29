using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSaaS.Persistence.Context;
using TaskManagementSaaS.Domain.Enums;
using TaskManagementSaaS.Domain.Interfaces;

namespace TaskManagementSaaS.Application.Commands.Tasks
{
    public record ClaimTaskCommand(Guid TaskId) : IRequest<bool>;

    public class ClaimTaskCommandHandler : IRequestHandler<ClaimTaskCommand, bool>
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;

        public ClaimTaskCommandHandler(AppDbContext context, IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<bool> Handle(ClaimTaskCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetUserId();
            if (!userId.HasValue) return false;

            var task = await _context.Tasks.FindAsync(new object[] { request.TaskId }, cancellationToken);
            if (task == null) return false;

            if (task.Status != TaskItemStatus.Open && task.Status != TaskItemStatus.Claimed) return false;

            task.AssignedUserId = userId.Value;
            task.Status = TaskItemStatus.InProgress;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
