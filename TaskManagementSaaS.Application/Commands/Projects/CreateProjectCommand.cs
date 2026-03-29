using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSaaS.Persistence.Context;
using TaskManagementSaaS.Domain.Entities;
using TaskManagementSaaS.Domain.Interfaces;

namespace TaskManagementSaaS.Application.Commands.Projects
{
    public record CreateProjectCommand(string Name) : IRequest<Guid>;

    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Guid>
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;

        public CreateProjectCommandHandler(AppDbContext context, IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _userContext.GetTenantId();
            if (tenantId == null) throw new UnauthorizedAccessException("No tenant context found.");

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                TenantId = tenantId.Value
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync(cancellationToken);

            return project.Id;
        }
    }
}
