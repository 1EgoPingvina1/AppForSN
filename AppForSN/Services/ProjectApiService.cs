using AppForSNForUsers.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppForSNForUsers.Services
{
    public class ProjectApiService
    {
        private readonly HttpClient _client;

        public ProjectApiService(string token)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            _client.BaseAddress = new Uri("https://localhost:5080/");
        }

        public async Task<List<ProjectViewDTO>> GetUserProjectsAsync()
        {
            var response = await _client.GetAsync("api/Project/my-projects");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ProjectViewDTO>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
