using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Shared.Services;

namespace MauiBlazorWeb.Services
{
    public class MauiShowClassService : IShowClassService
    {
        private readonly HttpClient _httpClient;

        public MauiShowClassService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ShowClassDto>> GetClassesByShowIdAsync(string showId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ShowClassDto>>($"api/showclasses?showId={showId}") 
                ?? new List<ShowClassDto>();
        }

        public async Task<ShowClassDto?> GetShowClassByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<ShowClassDto>($"api/showclasses/{id}");
        }

        public async Task<ShowClassDto> CreateShowClassAsync(ShowClassDto showClassDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/showclasses", showClassDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ShowClassDto>() ?? new ShowClassDto();
        }

        public async Task<ShowClassDto> UpdateShowClassAsync(ShowClassDto showClassDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/showclasses/{showClassDto.Id}", showClassDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ShowClassDto>() ?? new ShowClassDto();
        }

        public async Task<bool> DeleteShowClassAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/showclasses/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}