using System.Net.Http.Json;
using System.Text.Json;
using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Shared.Services;

namespace MauiBlazorWeb.Services
{
    public class MauiDivisionService : IDivisionService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IErrorHandler _errorHandler;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public MauiDivisionService(IHttpClientFactory httpClientFactory, IErrorHandler errorHandler)
        {
            _httpClientFactory = httpClientFactory;
            _errorHandler = errorHandler;
        }

        public async Task<IEnumerable<DivisionDto>> GetAllByShowIdAsync(string showId) =>
            await ExecuteAsync(async client =>
            {
                var resp = await client.GetAsync($"api/divisions?showId={showId}");
                if (!resp.IsSuccessStatusCode) return new List<DivisionDto>();
                var content = await resp.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<DivisionDto>>(content, _jsonOptions) ?? new List<DivisionDto>();
            }, new List<DivisionDto>(), nameof(GetAllByShowIdAsync));

        public async Task<DivisionDto?> GetByIdAsync(string id) =>
            await ExecuteAsync(async client =>
            {
                return await client.GetFromJsonAsync<DivisionDto>($"api/divisions/{id}", _jsonOptions);
            }, default(DivisionDto), nameof(GetByIdAsync));

        public async Task<DivisionDto> CreateDivisionAsync(DivisionDto divisionDto) =>
            await ExecuteAsync(async client =>
            {
                var resp = await client.PostAsJsonAsync("api/divisions", divisionDto);
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DivisionDto>(content, _jsonOptions) ?? new DivisionDto();
            }, new DivisionDto(), nameof(CreateDivisionAsync));

        public async Task<DivisionDto> UpdateDivisionAsync(DivisionDto divisionDto) =>
            await ExecuteAsync(async client =>
            {
                var resp = await client.PutAsJsonAsync($"api/divisions/{divisionDto.Id}", divisionDto);
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DivisionDto>(content, _jsonOptions) ?? new DivisionDto();
            }, new DivisionDto(), nameof(UpdateDivisionAsync));

        public async Task<bool> DeleteDivisionAsync(string id) =>
            await ExecuteAsync(async client =>
            {
                var resp = await client.DeleteAsync($"api/divisions/{id}");
                return resp.IsSuccessStatusCode;
            }, false, nameof(DeleteDivisionAsync));

        private async Task<T> ExecuteAsync<T>(Func<HttpClient, Task<T>> apiCall, T defaultValue, string methodName)
        {
            try
            {
                var client = await _httpClientFactory.CreateAuthenticatedClientAsync();
                return await apiCall(client);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex, $"Division API error in {methodName}: {ex.Message}");
                return defaultValue;
            }
        }
    }
}