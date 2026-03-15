//Global error handling 
using System.Net;
using System.Text.Json;

namespace TaskManagementSaaS.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        public static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var response = JsonSerializer.Serialize(new
            {
                message = "An unexpected error occurred.",
                details = exception.Message
            });
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
// any unhandled exception -> Json response