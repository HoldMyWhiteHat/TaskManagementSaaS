using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using TaskManagementSaaS.API.Middleware;
using TaskManagementSaaS.Infrastructure;
using TaskManagementSaaS.Persistence;
using TaskManagementSaaS.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();

// Swagger (Development only - configured below in pipeline)
builder.Services.AddSwaggerGen();

// Core Layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddScoped<Microsoft.AspNetCore.Authentication.IClaimsTransformation, TaskManagementSaaS.API.Infrastructure.RoleClaimsTransformation>();

// Auth0 Authentication with hardened JWT validation
var auth0Authority = builder.Configuration["Auth0:Authority"] ?? "https://dev-5rjxr6k61ht2n50r.us.auth0.com/";
var auth0Audience = builder.Configuration["Auth0:Audience"] ?? "TaskManagementSaaS.API";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = auth0Authority;
        options.Audience = auth0Audience;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = auth0Authority,
            ValidateAudience = true,
            ValidAudience = auth0Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2), // Tight clock skew (default is 5 min)
            RequireExpirationTime = true,
            RequireSignedTokens = true
        };

        // Reject tokens on validation failure
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning("Authentication failed: {Message}", context.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });

// CORS - Restricted policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("SecurePolicy", policy =>
    {
        var allowedOrigins = builder.Configuration["CORS:AllowedOrigins"]?.Split(',') 
            ?? new[] { "https://localhost:7094" };

        policy.WithOrigins(allowedOrigins)
              .WithMethods("GET", "POST", "PUT", "DELETE") // Only needed HTTP methods
              .WithHeaders("Authorization", "Content-Type", "Accept")
              .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
});

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Global fixed window rate limit
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 10;
    });

    // Stricter limit for auth endpoints (prevent brute force)
    options.AddFixedWindowLimiter("auth", limiterOptions =>
    {
        limiterOptions.PermitLimit = 20;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 5;
    });

    // On rejection, log the attempt
    options.OnRejected = async (context, cancellationToken) =>
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogWarning("Rate limit exceeded for {Path} from {IP}", 
            context.HttpContext.Request.Path, 
            context.HttpContext.Connection.RemoteIpAddress);

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
    };
});

var app = builder.Build();

// ===== MIDDLEWARE PIPELINE (order matters!) =====

// 1. Global Exception Handler (catches all unhandled errors)
app.UseMiddleware<ExceptionMiddleware>();

// 2. Security Headers (added to every response)
app.UseMiddleware<SecurityHeadersMiddleware>();

// 3. HTTPS Redirect
app.UseHttpsRedirection();

// 4. HSTS for production (tells browsers to only use HTTPS)
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// 5. Swagger (Development only!)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 6. CORS
app.UseCors("SecurePolicy");

// 7. Rate Limiting
app.UseRateLimiter();

// 8. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireRateLimiting("fixed");

app.Run();
