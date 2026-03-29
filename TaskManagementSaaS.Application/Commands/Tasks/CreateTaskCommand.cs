using MediatR;
using TaskManagementSaaS.Persistence.Context;
using TaskManagementSaaS.Domain.Entities;
using TaskManagementSaaS.Domain.Enums;

namespace TaskManagementSaaS.Application.Commands.Tasks
{
    public record CreateTaskCommand(string Title, string Description, TaskPriority Priority, Guid ProjectId) : IRequest<Guid>;

    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
    {
        private readonly AppDbContext _context;

        public CreateTaskCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects.FindAsync(new object[] { request.ProjectId }, cancellationToken);
            if (project == null) throw new Exception("Project not found");

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                Status = TaskItemStatus.Open,
                ProjectId = request.ProjectId,
                TenantId = project.TenantId
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync(cancellationToken);

            return task.Id;
        }
    }
}
