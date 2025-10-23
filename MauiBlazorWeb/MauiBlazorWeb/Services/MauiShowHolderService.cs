using System.Net.Http.Json;
using System.Text.Json;
using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Shared.Services;

namespace MauiBlazorWeb.Services
{
    public class MauiShowHolderService : IShowHolderService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public MauiShowHolderService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // userId/isAdmin are derived server-side from JWT claims; parameters are ignored on the client
        public async Task<IEnumerable<ShowDto>> GetMyShowsAsync(string userId, bool isAdmin = false)
        {
            return await ExecuteAsync(async client =>
            {
                var resp = await client.GetAsync("api/showholder/shows");
                if (!resp.IsSuccessStatusCode) return new List<ShowDto>();
                var content = await resp.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<ShowDto>>(content, _jsonOptions) ?? new List<ShowDto>();
            }, new List<ShowDto>(), nameof(GetMyShowsAsync));
        }

        public async Task<ShowDto> CreateShowAsync(string userId, ShowDto showDto, bool isAdmin = false)
        {
            return await ExecuteAsync(async client =>
            {
                var resp = await client.PostAsJsonAsync("api/showholder/shows", showDto);
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ShowDto>(content, _jsonOptions) ?? new ShowDto();
            }, new ShowDto(), nameof(CreateShowAsync));
        }

        public async Task<ShowDto> UpdateShowAsync(string userId, ShowDto showDto, bool isAdmin = false)
        {
            return await ExecuteAsync(async client =>
            {
                var resp = await client.PutAsJsonAsync($"api/showholder/shows/{showDto.Id}", showDto);
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ShowDto>(content, _jsonOptions) ?? new ShowDto();
            }, new ShowDto(), nameof(UpdateShowAsync));
        }

        public async Task<bool> DeleteShowAsync(string userId, string showId, bool isAdmin = false)
        {
            return await ExecuteAsync(async client =>
            {
                var resp = await client.DeleteAsync($"api/showholder/shows/{showId}");
                return resp.IsSuccessStatusCode;
            }, false, nameof(DeleteShowAsync));
        }

        private async Task<T> ExecuteAsync<T>(Func<HttpClient, Task<T>> apiCall, T defaultValue, string methodName)
        {
            try
            {
                // This extension is already used in MauiDataService to attach the JWT automatically
                var client = await _httpClientFactory.CreateAuthenticatedClientAsync();
                return await apiCall(client);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}