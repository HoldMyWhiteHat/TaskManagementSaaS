using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TaskManagementSaaS.Application.Commands.Auth;

namespace TaskManagementSaaS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableRateLimiting("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class SyncRequestDto
        {
            public string? Email { get; set; }
            public string? Name { get; set; }
            public string? TenantName { get; set; }
        }

        [HttpPost("sync")]
        public async Task<IActionResult> Sync([FromBody] SyncRequestDto? dto)
        {
            var subjectId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(subjectId)) return Unauthorized();

            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email") ?? dto?.Email ?? "";
            var name = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("name") ?? dto?.Name ?? email;

            var result = await _mediator.Send(new SyncUserCommand(subjectId, email, name, dto?.TenantName));

            if (result.RequiresTenantName)
            {
                return StatusCode(428, new { message = "TenantNameRequired" }); 
            }

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(new
            {
                Role = result.Role,
                TenantId = result.TenantId,
                TenantName = result.TenantName,
                RequiresTenantName = result.RequiresTenantName
            });
        }
    }
}