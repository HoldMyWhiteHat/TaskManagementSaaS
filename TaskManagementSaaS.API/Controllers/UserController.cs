using Microsoft.AspNetCore.Mvc;
using TaskManagementSaaS.Domain.Entities;

namespace TaskManagementSaaS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private static readonly List<User> _users = new List<User>();

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public IActionResult Create(string username, string email, string passwordHash, Guid tenantId)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                TenantId = tenantId,
                CreatedAt = DateTime.UtcNow
            };

            _users.Add(user);

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, string username, string email)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

            user.Username = username;
            user.Email = email;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

            _users.Remove(user);

            return NoContent();
        }
    }
}