using Microsoft.AspNetCore.Mvc;
using TaskManagementSaaS.Domain.Entities;

namespace TaskManagementSaaS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private static readonly List<Project> _projects = new List<Project>();

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_projects);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var project = _projects.FirstOrDefault(p => p.Id == id);

            if (project == null)
                return NotFound();

            return Ok(project);
        }

        [HttpPost]
        public IActionResult Create(Project project)
        {
            project.Id = _projects.Count + 1;

            _projects.Add(project);

            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Project updatedProject)
        {
            var project = _projects.FirstOrDefault(p => p.Id == id);

            if (project == null)
                return NotFound();

            project.Name = updatedProject.Name;
            project.TenantId = updatedProject.TenantId;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var project = _projects.FirstOrDefault(p => p.Id == id);

            if (project == null)
                return NotFound();

            _projects.Remove(project);

            return NoContent();
        }
    }
}