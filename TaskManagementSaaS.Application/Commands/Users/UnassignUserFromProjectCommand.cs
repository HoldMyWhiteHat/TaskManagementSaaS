using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSaaS.Persistence.Context;

namespace TaskManagementSaaS.Application.Commands.Users
{
    public record UnassignUserFromProjectCommand(Guid UserId, Guid ProjectId) : IRequest<bool>;

    public class UnassignUserFromProjectCommandHandler : IRequestHandler<UnassignUserFromProjectCommand, bool>
    {
        private readonly AppDbContext _context;

        public UnassignUserFromProjectCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UnassignUserFromProjectCommand request, CancellationToken cancellationToken)
        {
            var assignment = await _context.ProjectUsers
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(pu => pu.UserId == request.UserId && pu.ProjectId == request.ProjectId, cancellationToken);

            if (assignment == null) return false;

            _context.ProjectUsers.Remove(assignment);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
