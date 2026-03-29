using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSaaS.Application.DTO.Users;
using TaskManagementSaaS.Application.Queries.Users;
using TaskManagementSaaS.Application.Commands.Users;

namespace TaskManagementSaaS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            return await _mediator.Send(new GetAllUsersQuery());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Guid>> Create(CreateUserDto dto)
        {
            return await _mediator.Send(new CreateUserCommand(dto.Username, dto.Email, dto.Role));
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMe()
        {
            var subjectId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(subjectId)) return Unauthorized();

            var result = await _mediator.Send(new DeleteMyAccountCommand(subjectId));
            if (!result) return NotFound();
            return Ok();
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid userId)
        {
            var result = await _mediator.Send(new DeleteUserCommand(userId));
            if (!result) return NotFound();
            return Ok();
        }

        [HttpDelete("{userId}/project/{projectId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Unassign(Guid userId, Guid projectId)
        {
            var result = await _mediator.Send(new UnassignUserFromProjectCommand(userId, projectId));
            if (!result) return NotFound();
            return Ok();
        }
    }
}