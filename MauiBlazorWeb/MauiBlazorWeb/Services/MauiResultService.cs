using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Shared.Services;

namespace MauiBlazorWeb.Services
{
    public class MauiResultService : IResultService
    {
        private readonly HttpClient _httpClient;

        public MauiResultService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResultDto?> GetResultByEntryIdAsync(string entryId)
        {
            return await _httpClient.GetFromJsonAsync<ResultDto>($"api/results/entry/{entryId}");
        }

        public async Task<ResultDto?> GetResultByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<ResultDto>($"api/results/{id}");
        }

        public async Task<ResultDto> CreateResultAsync(ResultDto resultDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/results", resultDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ResultDto>() ?? new ResultDto();
        }

        public async Task<ResultDto> UpdateResultAsync(ResultDto resultDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/results/{resultDto.Id}", resultDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ResultDto>() ?? new ResultDto();
        }

        public async Task<bool> DeleteResultAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/results/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}