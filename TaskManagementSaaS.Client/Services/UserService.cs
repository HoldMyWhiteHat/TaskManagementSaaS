using System.Net.Http.Json;
using TaskManagementSaaS.Application.DTO.Users;

namespace TaskManagementSaaS.Client.Services
{
    public class UserService
    {
        private readonly HttpClient _http;

        public UserService(HttpClient http)
        {
            _http = http;
        }

        public class SyncUserResponse
        {
            public string? Role { get; set; }
            public Guid? TenantId { get; set; }
            public string? TenantName { get; set; }
            public bool RequiresTenantName { get; set; }
        }

        public async Task<SyncUserResponse?> SyncUserAsync(string? email, string? name, string? tenantName = null)
        {
            var response = await _http.PostAsJsonAsync("api/auth/sync", new { Email = email, Name = name, TenantName = tenantName });
            if (response.StatusCode == System.Net.HttpStatusCode.PreconditionRequired)
            {
                return new SyncUserResponse { RequiresTenantName = true };
            }
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<SyncUserResponse>();
        }

        private async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new Exception($"Status {response.StatusCode}: {content}");
            }
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _http.GetFromJsonAsync<List<UserDto>>("api/user") ?? new();
        }

        public async Task<Guid?> CreateUserAsync(CreateUserDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/user", dto);
            await EnsureSuccessAsync(response);
            return await response.Content.ReadFromJsonAsync<Guid>();
        }

        public async Task DeleteUserAsync(Guid id) =>
            await EnsureSuccessAsync(await _http.DeleteAsync($"api/user/{id}"));

        public async Task DeleteMyAccountAsync() =>
            await EnsureSuccessAsync(await _http.DeleteAsync("api/user/me"));

        public async Task UnassignFromProjectAsync(Guid userId, Guid projectId) =>
            await EnsureSuccessAsync(await _http.DeleteAsync($"api/user/{userId}/project/{projectId}"));
    }
}
