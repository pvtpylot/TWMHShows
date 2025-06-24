using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Shared.Services;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace MauiBlazorWeb.Services
{
    public class MauiRoleService : IRoleService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorage _tokenStorage;
        
        public MauiRoleService(HttpClient httpClient, ITokenStorage tokenStorage)
        {
            _httpClient = httpClient;
            _tokenStorage = tokenStorage;
        }

        public async Task<bool> AddUserToRoleAsync(string userId, string role)
        {
            await EnsureAuthTokenAsync();
            
            var response = await _httpClient.PostAsync(
                $"api/users/{userId}/roles?role={Uri.EscapeDataString(role)}", null);
                
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<string>> GetAllRolesAsync()
        {
            await EnsureAuthTokenAsync();
            
            return await _httpClient.GetFromJsonAsync<IEnumerable<string>>("api/roles") 
                ?? Array.Empty<string>();
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            await EnsureAuthTokenAsync();
            
            return await _httpClient.GetFromJsonAsync<IEnumerable<string>>($"api/users/{userId}/roles") 
                ?? Array.Empty<string>();
        }

        public async Task<bool> RemoveUserFromRoleAsync(string userId, string role)
        {
            await EnsureAuthTokenAsync();
            
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/users/{userId}/roles/{role}");
            var response = await _httpClient.SendAsync(request);
                
            return response.IsSuccessStatusCode;
        }
        
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            await EnsureAuthTokenAsync();
            
            return await _httpClient.GetFromJsonAsync<List<UserDto>>("api/users") 
                ?? new List<UserDto>();
        }
        
        public async Task<UserDto?> GetUserByIdAsync(string userId)
        {
            await EnsureAuthTokenAsync();
            
            return await _httpClient.GetFromJsonAsync<UserDto>($"api/users/{userId}");
        }
        
        private async Task EnsureAuthTokenAsync()
        {
            var token = await _tokenStorage.GetTokenAsync();
            if (token == null)
            {
                return;
            }
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.ToString());
        }
    }
}