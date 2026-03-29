using MediatR;
using TaskManagementSaaS.Domain.Entities;
using TaskManagementSaaS.Domain.Interfaces;
using TaskManagementSaaS.Persistence.Context;

namespace TaskManagementSaaS.Application.Commands.Users
{
    public record CreateUserCommand(string Username, string Email, string Role) : IRequest<Guid>;

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;

        public CreateUserCommandHandler(AppDbContext context, IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _userContext.GetTenantId();
            if (tenantId == null) throw new UnauthorizedAccessException("No tenant context found.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                Role = request.Role,
                TenantId = tenantId.Value,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }
}
