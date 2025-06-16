using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Shared.Services;

namespace MauiBlazorWeb.Services
{
    public class MauiDataService : IDataService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        private readonly MauiAuthenticationStateProvider _authStateProvider;

        public MauiDataService(HttpClient httpClient, MauiAuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
        }

        private async Task<HttpClient> GetAuthenticatedHttpClientAsync()
        {
            var accessTokenInfo = await _authStateProvider.GetAccessTokenInfoAsync();
            if (accessTokenInfo == null)
            {
                Debug.WriteLine("Authentication token is null or invalid");
                throw new UnauthorizedAccessException("User is not authenticated. Please log in.");
            }

            var token = accessTokenInfo.LoginResponse.AccessToken;
            var scheme = accessTokenInfo.LoginResponse.TokenType;

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue(scheme, token);

            return _httpClient;
        }

        public async Task<IEnumerable<UserModelObjectDto>> GetAllUserModelObjectsAsync(string? applicationUserId)
        {
            if (string.IsNullOrEmpty(applicationUserId))
                return new List<UserModelObjectDto>();
            
            try
            {
                var client = await GetAuthenticatedHttpClientAsync();
                Debug.WriteLine($"Making API request to: /api/userModelObjects?applicationUserId={applicationUserId}");
                
                var result = await client.GetFromJsonAsync<IEnumerable<UserModelObjectDto>>(
                    $"api/userModelObjects?applicationUserId={applicationUserId}", _jsonOptions);
                
                Debug.WriteLine($"API request successful, found {result?.Count() ?? 0} items");
                return result ?? new List<UserModelObjectDto>();
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine($"Authorization error: {ex.Message}");
                return new List<UserModelObjectDto>();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP request error in GetAllUserModelObjectsAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return new List<UserModelObjectDto>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error in GetAllUserModelObjectsAsync: {ex.Message}");
                return new List<UserModelObjectDto>();
            }
        }

        public async Task<UserModelObjectDto> GetUserModelObjectByIdAsync(string id)
        {
            try
            {
                var client = await GetAuthenticatedHttpClientAsync();
                Debug.WriteLine($"Making API request to: /api/userModelObjects/{id}");
                
                var result = await client.GetFromJsonAsync<UserModelObjectDto>(
                    $"api/userModelObjects/{id}", _jsonOptions);
                
                Debug.WriteLine("API request successful");
                return result ?? new UserModelObjectDto();
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine($"Authorization error: {ex.Message}");
                return new UserModelObjectDto();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP request error in GetUserModelObjectByIdAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return new UserModelObjectDto();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error in GetUserModelObjectByIdAsync: {ex.Message}");
                return new UserModelObjectDto();
            }
        }

        public async Task<UserModelObjectDto> CreateUserModelObjectAsync(UserModelObjectDto userModelObjectDto)
        {
            try
            {
                var client = await GetAuthenticatedHttpClientAsync();
                Debug.WriteLine("Making POST request to: /api/userModelObjects");
                Debug.WriteLine($"Request payload: {JsonSerializer.Serialize(userModelObjectDto)}");
                
                var response = await client.PostAsJsonAsync("api/userModelObjects", userModelObjectDto);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Response status code: {(int)response.StatusCode} {response.StatusCode}");
                Debug.WriteLine($"Response content: {responseContent}");
                
                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<UserModelObjectDto>(responseContent, _jsonOptions) ?? 
                           new UserModelObjectDto();
                }
                
                Debug.WriteLine($"Error creating model object: {responseContent}");
                return new UserModelObjectDto();
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine($"Authorization error: {ex.Message}");
                return new UserModelObjectDto();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP request error in CreateUserModelObjectAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return new UserModelObjectDto();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error in CreateUserModelObjectAsync: {ex.Message}");
                return new UserModelObjectDto();
            }
        }

        public async Task<UserModelObjectDto> UpdateUserModelObjectAsync(string id, UserModelObjectDto userModelObjectDto)
        {
            try
            {
                var client = await GetAuthenticatedHttpClientAsync();
                Debug.WriteLine($"Making PUT request to: /api/userModelObjects/{id}");
                Debug.WriteLine($"Request payload: {JsonSerializer.Serialize(userModelObjectDto)}");
                
                var response = await client.PutAsJsonAsync($"api/userModelObjects/{id}", userModelObjectDto);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Response status code: {(int)response.StatusCode} {response.StatusCode}");
                Debug.WriteLine($"Response content: {responseContent}");
                
                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<UserModelObjectDto>(responseContent, _jsonOptions) ?? 
                           new UserModelObjectDto();
                }
                
                Debug.WriteLine($"Error updating model object: {responseContent}");
                return new UserModelObjectDto();
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine($"Authorization error: {ex.Message}");
                return new UserModelObjectDto();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP request error in UpdateUserModelObjectAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return new UserModelObjectDto();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error in UpdateUserModelObjectAsync: {ex.Message}");
                return new UserModelObjectDto();
            }
        }

        public async Task<bool> DeleteUserModelObjectAsync(string id)
        {
            try
            {
                var client = await GetAuthenticatedHttpClientAsync();
                Debug.WriteLine($"Making DELETE request to: /api/userModelObjects/{id}");
                
                var response = await client.DeleteAsync($"api/userModelObjects/{id}");
                
                Debug.WriteLine($"Response status code: {(int)response.StatusCode} {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine($"Authorization error: {ex.Message}");
                return false;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP request error in DeleteUserModelObjectAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error in DeleteUserModelObjectAsync: {ex.Message}");
                return false;
            }
        }
    }
}