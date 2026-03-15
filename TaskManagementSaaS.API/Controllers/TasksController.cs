using Microsoft.AspNetCore.Mvc;
using TaskManagementSaaS.Domain.Entities;

namespace TaskManagementSaaS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private static readonly List<TaskItem> _tasks = new List<TaskItem>();

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_tasks);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost]
        public IActionResult Create(TaskItem task)
        {
            task.Id = Guid.NewGuid();
            task.CreatedAt = DateTime.UtcNow;

            _tasks.Add(task);

            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, TaskItem updatedTask)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
                return NotFound();

            task.Title = updatedTask.Title;
            task.Description = updatedTask.Description;
            task.IsCompleted = updatedTask.IsCompleted;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
                return NotFound();

            _tasks.Remove(task);

            return NoContent();
        }
    }
}