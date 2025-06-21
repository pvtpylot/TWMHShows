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
                return new List<UserModelObjectDto>();
            
            return await ExecuteApiRequestAsync(
                async (client) => 
                {
                    Debug.WriteLine($"Making API request to: /api/userModelObjects?applicationUserId={applicationUserId}");
                    var result = await client.GetFromJsonAsync<IEnumerable<UserModelObjectDto>>(
                        $"api/userModelObjects?applicationUserId={applicationUserId}", _jsonOptions);
                    
                    Debug.WriteLine($"API request successful, found {result?.Count() ?? 0} items");
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
                    Debug.WriteLine($"Making API request to: /api/userModelObjects/{id}");
                    var result = await client.GetFromJsonAsync<UserModelObjectDto>(
                        $"api/userModelObjects/{id}", _jsonOptions);
                    
                    Debug.WriteLine("API request successful");
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
                    Debug.WriteLine("Making POST request to: /api/userModelObjects");
                    Debug.WriteLine($"Request payload: {JsonSerializer.Serialize(userModelObjectDto, _jsonOptions)}");
                    
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
                },
                new UserModelObjectDto(),
                nameof(CreateUserModelObjectAsync));
        }

        public async Task<UserModelObjectDto> UpdateUserModelObjectAsync(string id, UserModelObjectDto userModelObjectDto)
        {
            return await ExecuteApiRequestAsync(
                async (client) => 
                {
                    Debug.WriteLine($"Making PUT request to: /api/userModelObjects/{id}");
                    Debug.WriteLine($"Request payload: {JsonSerializer.Serialize(userModelObjectDto, _jsonOptions)}");
                    
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
                },
                new UserModelObjectDto(),
                nameof(UpdateUserModelObjectAsync));
        }

        public async Task<bool> DeleteUserModelObjectAsync(string id)
        {
            return await ExecuteApiRequestAsync(
                async (client) => 
                {
                    Debug.WriteLine($"Making DELETE request to: /api/userModelObjects/{id}");
                    
                    var response = await client.DeleteAsync($"api/userModelObjects/{id}");
                    
                    Debug.WriteLine($"Response status code: {(int)response.StatusCode} {response.StatusCode}");
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