using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Shared.Services;

namespace MauiBlazorWeb.Services
{
    public class MauiShowService : IShowService
    {
        private readonly HttpClient _httpClient;

        public MauiShowService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ShowDto>> GetAllShowsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ShowDto>>("api/shows") 
                ?? new List<ShowDto>();
        }

        public async Task<IEnumerable<ShowDto>> GetShowsByJudgeIdAsync(string judgeId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ShowDto>>($"api/shows/judge/{judgeId}") 
                ?? new List<ShowDto>();
        }

        public async Task<ShowDto?> GetShowByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<ShowDto>($"api/shows/{id}");
        }

        public async Task<ShowDto> CreateShowAsync(ShowDto showDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/shows", showDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ShowDto>() ?? new ShowDto();
        }

        public async Task<ShowDto> UpdateShowAsync(ShowDto showDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/shows/{showDto.Id}", showDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ShowDto>() ?? new ShowDto();
        }

        public async Task<bool> DeleteShowAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/shows/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}