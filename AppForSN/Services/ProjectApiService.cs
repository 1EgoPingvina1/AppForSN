using AppForSNForUsers.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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

        public async Task<List<ProjectResponseDto>> GetAllProjectsAsync()
        {
            //var response = await _client.GetFromJsonAsync<ProjectResponseDto>("api/Project/my-projects");
            //response.EnsureSuccessStatusCode();

            //var json = await response.Content.ReadAsStringAsync();
            //return JsonSerializer.Deserialize<List<ProjectViewDTO>>(json, new JsonSerializerOptions
            //{
            //    PropertyNameCaseInsensitive = true
            //});


            try
            {
                return new List<ProjectResponseDto>
                 {
                     new ProjectResponseDto { Name = "KartunCat Tells About the World", Version = "1.0", Status = "Approved" },
                     new ProjectResponseDto { Name = "Super Ivan", Version = "0.8", Status = "In Review" }
                 };
            }
            catch (HttpRequestException ex)
            {
                // Логирование ошибки
                return new List<ProjectResponseDto>(); // Возвращаем пустой список в случае ошибки
            }
        }
    }
}
