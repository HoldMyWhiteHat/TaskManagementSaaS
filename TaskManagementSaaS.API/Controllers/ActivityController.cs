using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSaaS.Application.Queries.Activity;
using TaskManagementSaaS.Application.DTO.Activity;

namespace TaskManagementSaaS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ActivityController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ActivityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("recent")]
        public async Task<ActionResult<List<ActivityLogDto>>> GetRecent([FromQuery] int count = 50)
        {
            return await _mediator.Send(new GetRecentActivityQuery(count));
        }
    }
}
