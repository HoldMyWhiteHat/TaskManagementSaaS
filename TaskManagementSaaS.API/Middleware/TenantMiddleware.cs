namespace TaskManagementSaaS.API.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantId))
            {
                context.Items["TenantId"] = tenantId.ToString();
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Tenant ID is required in the X-Tenant-ID header.");
                return;
            }
            await _next(context);
        }
    }
}

// Tenant a sees only tenant a projects
// Tenant b sees only tenant b projents, so now tenant isolation is achieved