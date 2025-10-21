using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Shared.Services;

namespace MauiBlazorWeb.Services
{
    public class MauiDataService : IDataService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        private readonly IErrorHandler _errorHandler;

        public MauiDataService(IHttpClientFactory httpClientFactory, IErrorHandler errorHandler)
        {
            _httpClientFactory = httpClientFactory;
            _errorHandler = errorHandler;
        }

        public async Task<IEnumerable<UserModelObjectDto>> GetAllUserModelObjectsAsync(string? applicationUserId)
        {
            if (string.IsNullOrEmpty(applicationUserId))
            {
                Debug.WriteLine("[MauiDataService] GetAllUserModelObjectsAsync: applicationUserId is null or empty");
                return new List<UserModelObjectDto>();
            }
            
            return await ExecuteApiRequestAsync(
                async (client) => 
                {
                    // Note: The API endpoint gets userId from JWT claims, not from query params
                    // The applicationUserId parameter is just for client-side validation
                    var response = await client.GetAsync("api/userModelObjects");
                    var responseContent = await response.Content.ReadAsStringAsync();
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine($"[MauiDataService] Error response: {response.StatusCode}");
                        return new List<UserModelObjectDto>();
                    }
                    
                    var result = JsonSerializer.Deserialize<IEnumerable<UserModelObjectDto>>(responseContent, _jsonOptions);
                    return result ?? new List<UserModelObjectDto>();
                },
                new List<UserModelObjectDto>(),
                nameof(GetAllUserModelObjectsAsync));
        }

        public async Task<UserModelObjectDto> GetUserModelObjectByIdAsync(string id)
        {
            return await ExecuteApiRequestAsync(
                async (client) => 
                {
                    var result = await client.GetFromJsonAsync<UserModelObjectDto>(
                        $"api/userModelObjects/{id}", _jsonOptions);
                    
                    return result ?? new UserModelObjectDto();
                },
                new UserModelObjectDto(),
                nameof(GetUserModelObjectByIdAsync));
        }

        public async Task<UserModelObjectDto> CreateUserModelObjectAsync(UserModelObjectDto userModelObjectDto)
        {
            return await ExecuteApiRequestAsync(
                async (client) => 
                {
                    var response = await client.PostAsJsonAsync("api/userModelObjects", userModelObjectDto);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        return JsonSerializer.Deserialize<UserModelObjectDto>(responseContent, _jsonOptions) ?? 
                               new UserModelObjectDto();
                    }
                    
                    Debug.WriteLine($"[MauiDataService] Error creating model object: {response.StatusCode}");
                    return new UserModelObjectDto();
                },
                new UserModelObjectDto(),
                nameof(CreateUserModelObjectAsync));
        }

        public async Task<UserModelObjectDto> UpdateUserModelObjectAsync(string id, UserModelObjectDto userModelObjectDto)
        {
            return await ExecuteApiRequestAsync(
                async (client) => 
                {
                    var response = await client.PutAsJsonAsync($"api/userModelObjects/{id}", userModelObjectDto);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        return JsonSerializer.Deserialize<UserModelObjectDto>(responseContent, _jsonOptions) ?? 
                               new UserModelObjectDto();
                    }
                    
                    Debug.WriteLine($"[MauiDataService] Error updating model object: {response.StatusCode}");
                    return new UserModelObjectDto();
                },
                new UserModelObjectDto(),
                nameof(UpdateUserModelObjectAsync));
        }

        public async Task<bool> DeleteUserModelObjectAsync(string id)
        {
            return await ExecuteApiRequestAsync(
                async (client) => 
                {
                    var response = await client.DeleteAsync($"api/userModelObjects/{id}");
                    return response.IsSuccessStatusCode;
                },
                false,
                nameof(DeleteUserModelObjectAsync));
        }

        private async Task<T> ExecuteApiRequestAsync<T>(Func<HttpClient, Task<T>> apiCall, T defaultValue, string methodName)
        {
            try
            {
                var client = await _httpClientFactory.CreateAuthenticatedClientAsync();
                return await apiCall(client);
            }
            catch (UnauthorizedAccessException ex)
            {
                _errorHandler.HandleError(ex, $"Authorization error in {methodName}: {ex.Message}");
                return defaultValue;
            }
            catch (HttpRequestException ex)
            {
                _errorHandler.HandleError(ex, $"HTTP request error in {methodName}: {ex.Message}");
                return defaultValue;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex, $"Unexpected error in {methodName}: {ex.Message}");
                return defaultValue;
            }
        }
    }
}