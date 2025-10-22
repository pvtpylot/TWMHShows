using System.Net.Http.Json;
using MauiBlazorWeb.Shared.Models.DTOs;
using MauiBlazorWeb.Shared.Services;

namespace MauiBlazorWeb.Services;

public class MauiEntryService : IEntryService
{
    private readonly HttpClient _httpClient;

    public MauiEntryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<EntryDto>> GetEntriesByShowClassIdAsync(string showClassId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<EntryDto>>($"api/entries?showClassId={showClassId}")
               ?? new List<EntryDto>();
    }

    public async Task<IEnumerable<EntryDto>> GetEntriesByUserModelObjectIdAsync(string userModelObjectId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<EntryDto>>(
                   $"api/entries?userModelObjectId={userModelObjectId}")
               ?? new List<EntryDto>();
    }

    public async Task<EntryDto?> GetEntryByIdAsync(string id)
    {
        return await _httpClient.GetFromJsonAsync<EntryDto>($"api/entries/{id}");
    }

    public async Task<EntryDto> CreateEntryAsync(EntryDto entryDto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/entries", entryDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EntryDto>() ?? new EntryDto();
    }

    public async Task<EntryDto> UpdateEntryAsync(EntryDto entryDto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/entries/{entryDto.Id}", entryDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EntryDto>() ?? new EntryDto();
    }

    public async Task<bool> DeleteEntryAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/entries/{id}");
        return response.IsSuccessStatusCode;
    }
}