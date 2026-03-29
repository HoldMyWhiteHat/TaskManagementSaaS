using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSaaS.Application.Commands.Tasks;
using TaskManagementSaaS.Application.DTO.Tasks;
using TaskManagementSaaS.Application.Queries.Tasks;

namespace TaskManagementSaaS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskDto>>> GetAll([FromQuery] Guid? projectId)
        {
            return await _mediator.Send(new GetAllTasksQuery(projectId));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Guid>> Create(CreateTaskDto dto)
        {
            return await _mediator.Send(new CreateTaskCommand(dto.Title, dto.Description, dto.Priority, dto.ProjectId));
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> Complete(Guid id)
        {
            var result = await _mediator.Send(new CompleteTaskCommand(id));
            if (!result) return BadRequest("Could not complete task.");
            return Ok();
        }

        [HttpPost("{id}/claim")]
        public async Task<IActionResult> Claim(Guid id)
        {
            var result = await _mediator.Send(new ClaimTaskCommand(id));
            if (!result) return BadRequest("Could not claim task.");
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteTaskCommand(id));
            if (!result) return BadRequest("Only Admins can delete completed tasks, or task not found.");
            return Ok();
        }
    }
}