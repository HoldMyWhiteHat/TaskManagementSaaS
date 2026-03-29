using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSaaS.Application.Commands.Tenants;
using TaskManagementSaaS.Application.DTO.Tenants;
using TaskManagementSaaS.Application.Queries.Tenants;

namespace TaskManagementSaaS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class TenantsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TenantsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyTenant()
        {
            var result = await _mediator.Send(new GetMyWorkspaceQuery());
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut("my")]
        public async Task<IActionResult> Update([FromBody] UpdateTenantDto dto)
        {
            var result = await _mediator.Send(new UpdateWorkspaceCommand(dto.Name));
            if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
            return NoContent();
        }
    }
}