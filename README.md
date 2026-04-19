# Task Management SaaS

A multi-tenant task management web app where teams organize work into projects and tasks within isolated workspaces. 
Built for small teams that need a simple way to create tasks, assign them, and track progress without the overhead of larger tools like Jira.
The first user to sign up creates a workspace and becomes the Admin. Admins create projects, add tasks with priorities, and pre-create user accounts by email. 
When those users log in through Auth0, they get linked to the workspace automatically and can start claiming and completing tasks.

# Features

- Workspace isolation — each tenant's data is fully separated at the database level using EF Core global query filters
- Role-based access control — Admins can create projects/tasks/users, regular Users can view assigned projects and claim/complete tasks
- Task management — tasks have a title, description, priority (Low, Medium, High, Urgent), and status (Open, Claimed, InProgress, Completed)
- Project organization — group tasks under projects and assign users to specific projects
- Activity log — tracks actions across the workspace (task created, user added, etc.)
- Dashboard — shows project count, task stats, recent tasks, and activity feed
- Account management — users can delete their own account, admins can delete the whole workspace

# Tech Stack

- C# / .NET 8
- ASP.NET Core 8 (REST API)
- Blazor WebAssembly (frontend SPA)
- Entity Framework Core 8 (ORM)
- SQL Server / Azure SQL (database)
- MediatR (CQRS pattern)
- Auth0 (authentication via OIDC + JWT)
- Azure App Service (hosting)
- GitHub Actions (CI/CD)

# Installation

Prerequisites: .NET 8 SDK, SQL Server instance, Auth0 account.

1. Clone the repo

git clone https://github.com/HoldMyWhiteHat/TaskManagementSaaS.git
cd TaskManagementSaaS

2. Create TaskManagementSaaS.API/appsettings.json

This file is gitignored. You need to create it manually:

.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TaskManagementSaaS;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Auth0": {
    "Authority": "https://YOUR_AUTH0_DOMAIN/",
    "Audience": "TaskManagementSaaS.API"
  },
  "CORS": {
    "AllowedOrigins": "https://localhost:____"
  }
}


3. Set up Auth0

- Create a Single Page Application in Auth0 dashboard
- Set callback URL, logout URL, and allowed web origins to https://localhost:____
- Create an API with identifier TaskManagementSaaS.API
- Update the Auth0 domain and client ID in TaskManagementSaaS.Client/Program.cs

4. Apply migrations and run

dotnet ef database update --project TaskManagementSaaS.Persistence --startup-project TaskManagementSaaS.API
dotnet run --project TaskManagementSaaS.API

App runs at https://localhost:____. Swagger is available at /swagger in development mode.

# Usage

All API endpoints require authentication (JWT Bearer token). Every request is automatically scoped to the caller's tenant.

*Create a project (Admin only)*

POST /api/projects
Content-Type: application/json

{
  "name": "Backend Redesign"
}

Response: "3fa8f64-517-4562-b3fc-2cf6aa6" (project ID)

*Create a task (Admin only)*

POST /api/tasks
Content-Type: application/json

{
  "title": "Set up CI pipeline",
  "description": "Configure GitHub Actions for build and deploy",
  "priority": 2,
  "projectId": "3fa85f64-5717-562-bfc-2c963f6a6"
}

Priority values: 0 = Low, 1 = Medium, 2 = High, 3 = Urgent

*Claim a task*

POST /api/tasks/{taskId}/claim

*Complete a task*

POST /api/tasks/{taskId}/complete


*Pre-create a user account (Admin only)*

POST /api/user
Content-Type: application/json

{
  "username": "jane",
  "email": "jane@company.com",
  "role": "User"
}

Role must be "User" or "Admin".

*Other endpoints*

GET    /api/projects                         — list all projects
DELETE /api/projects/{id}                    — delete a project
POST   /api/projects/{id}/users/{userId}     — assign user to project
GET    /api/tasks                            — list all tasks
GET    /api/tasks?projectId={id}             — list tasks for a project
DELETE /api/tasks/{id}                       — delete a task
GET    /api/user                             — list workspace users
DELETE /api/user/{id}                        — delete a user
DELETE /api/user/me                          — delete own account
DELETE /api/user/{userId}/project/{projectId} — unassign user from project
GET    /api/activity/recent?count=10         — recent activity log
POST   /api/auth/sync                        — first-login user sync
GET    /api/tenants                          — tenant info


# Project Structure

TaskManagementSaaS.API/              — ASP.NET Core host, controllers, middleware (auth, security headers, exception handling)
TaskManagementSaaS.Client/           — Blazor WebAssembly SPA (pages, layouts, services)
TaskManagementSaaS.Application/      — Business logic layer, CQRS commands/queries, DTOs, MediatR handlers
TaskManagementSaaS.Domain/           — Domain entities (Tenant, User, Project, TaskItem, ActivityLog), enums, interfaces
TaskManagementSaaS.Infrastructure/   — Infrastructure services (user context resolution from JWT claims)
TaskManagementSaaS.Persistence/      — EF Core DbContext, entity configurations, migrations


The API project references the Client project and serves the Blazor app as static files using `UseBlazorFrameworkFiles()`, so everything deploys as a single unit.

# Configuration

*Auth0* — handles all authentication. The API validates JWT tokens issued by Auth0. The Blazor client uses OIDC with PKCE flow. Both need to point at the same Auth0 tenant and API identifier.

*SQL Server* — connection string goes in appsettings.json under ConnectionStrings:DefaultConnection. For Azure SQL, use the Azure-provided connection string.

*CORS* — configured in appsettings.json under CORS:AllowedOrigins. Set this to whatever domain the app is served from.

# Deployment

The app is deployed to Azure App Service. The GitHub Actions workflow (`.github/workflows/master_taskmanagementsaas.yml`) builds and deploys on every push to master.

Set these environment variables on the App Service:


ConnectionStrings__DefaultConnection=<your_azure-sql-connection-string>
Auth0__Authority=https://YOUR_AUTH0_DOMAIN/
Auth0__Audience=TaskManagementSaaS.API
CORS__AllowedOrigins=https://your-app.azurewebsites.net

Startup command: dotnet TaskManagementSaaS.API.dll
Remember to update Auth0's allowed callback URLs, logout URLs, and web origins to include your production domain.
