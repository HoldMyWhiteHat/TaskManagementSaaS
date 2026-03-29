using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSaaS.Application.DTO.Users;
using TaskManagementSaaS.Persistence.Context;

namespace TaskManagementSaaS.Application.Queries.Users
{
    public record GetAllUsersQuery() : IRequest<List<UserDto>>;

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
    {
        private readonly AppDbContext _context;

        public GetAllUsersQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    TenantId = u.TenantId
                })
                .ToListAsync(cancellationToken);
        }
    }
}
