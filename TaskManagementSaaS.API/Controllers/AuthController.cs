using Microsoft.AspNetCore.Mvc;
using TaskManagementSaaS.Domain.Entities;

namespace TaskManagementSaaS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private static readonly List<User> _users = new List<User>();

        [HttpPost("register")]
        public IActionResult Register(string username, string email, string password, Guid tenantId)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = password, // later we hash it
                TenantId = tenantId,
                CreatedAt = DateTime.UtcNow
            };

            _users.Add(user);

            return Ok(user);
        }

        [HttpPost("login")]
        public IActionResult Login(string email, string password)
        {
            var user = _users.FirstOrDefault(u =>
                u.Email == email &&
                u.PasswordHash == password);

            if (user == null)
                return Unauthorized("Invalid credentials");

            return Ok(new
            {
                message = "Login successful",
                userId = user.Id
            });
        }
    }
}