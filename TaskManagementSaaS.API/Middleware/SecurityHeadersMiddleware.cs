namespace TaskManagementSaaS.API.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Prevent MIME-type sniffing
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            // Prevent clickjacking
            context.Response.Headers.Append("X-Frame-Options", "DENY");

            // Enable XSS filtering in older browsers
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

            // Control referrer information
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            // Restrict browser features
            context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

            // Content Security Policy - Configured for Blazor WASM and Auth0
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-eval' 'unsafe-inline' 'wasm-unsafe-eval'; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdn.jsdelivr.net; font-src 'self' data: https://fonts.gstatic.com https://cdn.jsdelivr.net; connect-src 'self' https://dev-5rjxr6k61ht2n50r.us.auth0.com; img-src 'self' data: https:; frame-ancestors 'none';");

            // Prevent caching of sensitive data
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.Headers.Append("Cache-Control", "no-store, no-cache, must-revalidate");
                context.Response.Headers.Append("Pragma", "no-cache");
            }

            await _next(context);
        }
    }
}
