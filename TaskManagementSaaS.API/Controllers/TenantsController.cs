using Microsoft.AspNetCore.Mvc;
using TaskManagementSaaS.Domain.Entities;

namespace TaskManagementSaaS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantsController : ControllerBase
    {
        private static readonly List<Tenant> _tenants = new List<Tenant>();

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_tenants);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var tenant = _tenants.FirstOrDefault(t => t.Id == id);

            if (tenant == null)
                return NotFound();

            return Ok(tenant);
        }

        [HttpPost]
        public IActionResult Create(string name)
        {
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            _tenants.Add(tenant);

            return CreatedAtAction(nameof(GetById), new { id = tenant.Id }, tenant);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, string name)
        {
            var tenant = _tenants.FirstOrDefault(t => t.Id == id);

            if (tenant == null)
                return NotFound();

            tenant.Name = name;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var tenant = _tenants.FirstOrDefault(t => t.Id == id);

            if (tenant == null)
                return NotFound();

            _tenants.Remove(tenant);

            return NoContent();
        }
    }
}