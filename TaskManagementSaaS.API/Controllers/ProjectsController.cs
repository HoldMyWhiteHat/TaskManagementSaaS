using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSaaS.Application.Commands.Projects;
using TaskManagementSaaS.Application.DTO.Projects;
using TaskManagementSaaS.Application.Queries.Projects;

namespace TaskManagementSaaS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectDto>>> GetAll()
        {
            return await _mediator.Send(new GetAllProjectsQuery());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Guid>> Create(CreateProjectDto dto)
        {
            return await _mediator.Send(new CreateProjectCommand(dto.Name));
        }

        [HttpPost("{id}/users/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignUser(Guid id, Guid userId)
        {
            var result = await _mediator.Send(new AssignUserToProjectCommand(id, userId));
            if (!result) return NotFound();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteProjectCommand(id));
            if (!result) return NotFound();
            return Ok();
        }
    }
}