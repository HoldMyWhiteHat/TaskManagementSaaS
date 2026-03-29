using System.Net.Http.Json;
using TaskManagementSaaS.Application.DTO.Tasks;

namespace TaskManagementSaaS.Client.Services
{
    public class TaskService
    {
        private readonly HttpClient _http;

        public TaskService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<TaskDto>> GetAllAsync() => 
            await _http.GetFromJsonAsync<List<TaskDto>>("api/tasks") ?? new();

        public async Task<TaskDto?> GetByIdAsync(Guid id) => 
            await _http.GetFromJsonAsync<TaskDto>($"api/tasks/{id}");

        private async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new Exception($"Status {response.StatusCode}: {content}");
            }
        }

        public async Task CreateAsync(CreateTaskDto dto) => 
            await EnsureSuccessAsync(await _http.PostAsJsonAsync("api/tasks", dto));

        public async Task UpdateAsync(Guid id, UpdateTaskDto dto) =>
            await EnsureSuccessAsync(await _http.PutAsJsonAsync($"api/tasks/{id}", dto));

        public async Task DeleteAsync(Guid id) => 
            await EnsureSuccessAsync(await _http.DeleteAsync($"api/tasks/{id}"));

        public async Task ClaimAsync(Guid id) =>
            await EnsureSuccessAsync(await _http.PostAsync($"api/tasks/{id}/claim", null));

        public async Task CompleteAsync(Guid id) =>
            await EnsureSuccessAsync(await _http.PostAsync($"api/tasks/{id}/complete", null));

        public async Task UpdateStatusAsync(Guid id, string status) =>
            await EnsureSuccessAsync(await _http.PostAsync($"api/tasks/{id}/status?status={status}", null));
    }
}
