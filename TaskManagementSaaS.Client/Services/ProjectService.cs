using System.Net.Http.Json;
using TaskManagementSaaS.Application.DTO.Projects;

namespace TaskManagementSaaS.Client.Services
{
    public class ProjectService
    {
        private readonly HttpClient _http;

        public ProjectService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<ProjectDto>> GetAllAsync() => 
            await _http.GetFromJsonAsync<List<ProjectDto>>("api/projects") ?? new();

        public async Task<ProjectDto?> GetByIdAsync(Guid id) => 
            await _http.GetFromJsonAsync<ProjectDto>($"api/projects/{id}");

        private async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new Exception($"Status {response.StatusCode}: {content}");
            }
        }

        public async Task CreateAsync(CreateProjectDto dto) => 
            await EnsureSuccessAsync(await _http.PostAsJsonAsync("api/projects", dto));

        public async Task UpdateAsync(Guid id, UpdateProjectDto dto) =>
            await EnsureSuccessAsync(await _http.PutAsJsonAsync($"api/projects/{id}", dto));
            
        public async Task DeleteAsync(Guid id) => 
            await EnsureSuccessAsync(await _http.DeleteAsync($"api/projects/{id}"));

        public async Task AssignUserAsync(Guid projectId, Guid userId) =>
            await EnsureSuccessAsync(await _http.PostAsync($"api/projects/{projectId}/users/{userId}", null));

        public async Task UnassignUserAsync(Guid projectId, Guid userId) =>
            await EnsureSuccessAsync(await _http.DeleteAsync($"api/projects/{projectId}/users/{userId}"));
    }
}
