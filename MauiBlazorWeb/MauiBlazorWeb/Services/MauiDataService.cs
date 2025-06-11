using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Shared.Services;

namespace MauiBlazorWeb.Services
{
    public class MauiDataService : IDataService
    {
        private readonly HttpClient _httpClient;

        public MauiDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<UserModelObjectDto>> GetAllUserModelObjectsAsync(string? applicationUserId)
        {
            var queryString = applicationUserId != null ? $"?applicationUserId={applicationUserId}" : "";
            return await _httpClient.GetFromJsonAsync<IEnumerable<UserModelObjectDto>>($"api/userModelObjects{queryString}") 
                ?? new List<UserModelObjectDto>();
        }

        public async Task<UserModelObjectDto> GetUserModelObjectByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<UserModelObjectDto>($"api/userModelObjects/{id}") 
                ?? new UserModelObjectDto();
        }
    }
}