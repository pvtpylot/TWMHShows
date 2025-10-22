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

        public async Task<bool> CreateRoleAsync(string roleName)
        {
            await EnsureAuthTokenAsync();
            
            var response = await _httpClient.PostAsync(
                $"api/roles?name={Uri.EscapeDataString(roleName)}", null);
                
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateRoleAsync(string oldRoleName, string newRoleName)
        {
            await EnsureAuthTokenAsync();
            
            var response = await _httpClient.PutAsync(
                $"api/roles/{Uri.EscapeDataString(oldRoleName)}?newName={Uri.EscapeDataString(newRoleName)}", null);
                
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteRoleAsync(string roleName)
        {
            await EnsureAuthTokenAsync();
            
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/roles/{Uri.EscapeDataString(roleName)}");
            var response = await _httpClient.SendAsync(request);
                
            return response.IsSuccessStatusCode;
        }

        public async Task<int> GetUsersInRoleCountAsync(string roleName)
        {
            await EnsureAuthTokenAsync();
            
            try
            {
                return await _httpClient.GetFromJsonAsync<int>($"api/roles/{Uri.EscapeDataString(roleName)}/count");
            }
            catch
            {
                return 0;
            }
        }
        
        private async Task EnsureAuthTokenAsync()
        {
            var token = await _tokenStorage.GetTokenAsync();
            var accessToken = token?.LoginResponse?.AccessToken;

            _httpClient.DefaultRequestHeaders.Authorization = 
                !string.IsNullOrWhiteSpace(accessToken)
                ? new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken)
                : null;
        }
    }
}