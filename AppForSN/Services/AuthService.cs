using AppForSNForUsers.Contracts;
using AppForSNForUsers.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AppForSNForUsers.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<string> GetTokenAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAuthenticatedAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<UserDTO> LoginAsync(LoginDTO loginDTO)
        {
            var response = await _httpClient.PostAsJsonAsync("account/login", loginDTO);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Server returned error: {error}");
            }

            var user = await response.Content.ReadFromJsonAsync<UserDTO>();
            return user;
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
