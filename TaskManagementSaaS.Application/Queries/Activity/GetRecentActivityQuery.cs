using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSaaS.Application.DTO.Activity;
using TaskManagementSaaS.Persistence.Context;

namespace TaskManagementSaaS.Application.Queries.Activity
{
    public record GetRecentActivityQuery(int Count = 50) : IRequest<List<ActivityLogDto>>;

    public class GetRecentActivityQueryHandler : IRequestHandler<GetRecentActivityQuery, List<ActivityLogDto>>
    {
        private readonly AppDbContext _context;

        public GetRecentActivityQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ActivityLogDto>> Handle(GetRecentActivityQuery request, CancellationToken cancellationToken)
        {
            return await _context.ActivityLogs
                .OrderByDescending(a => a.Timestamp)
                .Take(request.Count)
                .Select(a => new ActivityLogDto
                {
                    Id = a.Id,
                    Message = a.Message,
                    UserEmail = a.UserEmail,
                    Timestamp = a.Timestamp
                })
                .ToListAsync(cancellationToken);
        }
    }
}
