using System.Net.Http.Json;
using TaskManagementSaaS.Application.DTO.Activity;

namespace TaskManagementSaaS.Client.Services
{
    public class ActivityService
    {
        private readonly HttpClient _http;

        public ActivityService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<ActivityLogDto>> GetRecentAsync(int count = 50) => 
            await _http.GetFromJsonAsync<List<ActivityLogDto>>($"api/activity/recent?count={count}") ?? new();
    }
}
